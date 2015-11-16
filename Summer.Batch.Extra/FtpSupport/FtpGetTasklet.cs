//
//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using NLog;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Common.Factory;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.FtpSupport
{
    /// <summary>
    /// Custom tasklet to do some ftp get
    /// NOTE : For now, sftp is UNsupported
    /// </summary>
    public class FtpGetTasklet : ITasklet, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Attributes
        /// <summary>
        /// Ftp server host property.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Ftp server port property.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Ftp server user property.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Ftp server password property.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Ftp local directory property.
        /// </summary>
        public string LocalDirectory { get; set; }

        /// <summary>
        /// File name pattern property (used for filtering).
        /// </summary>
        public string FileNamePattern { get; set; }

        /// <summary>
        /// Ftp remote directory property.
        /// </summary>
        public string RemoteDirectory { get; set; }
        
        private int _downloadFileAttempts = 12;

        /// <summary>
        /// maximum download file attempts property.
        /// </summary>
        public int DownloadFileAttempts
        {
            get { return _downloadFileAttempts; }
            set { _downloadFileAttempts = value; }
        }

        private bool _autoCreateLocalDirectory = true;

        /// <summary>
        /// Whether to create local directory automatically if required or not.
        /// </summary>
        public bool AutoCreateLocalDirectory
        {
            get { return _autoCreateLocalDirectory; }
            set { _autoCreateLocalDirectory = value; }
        }

        private bool _deleteLocalFiles = true;

        /// <summary>
        /// Whether to delete local files automatically or not.
        /// </summary>
        public bool DeleteLocalFiles
        {
            get { return _deleteLocalFiles; }
            set { _deleteLocalFiles = value; }
        }

        private long _retryIntervalMilliseconds = 300000;

        /// <summary>
        /// Time in ms to wait for a retry.
        /// </summary>
        public long RetryIntervalMilliseconds
        {
            get { return _retryIntervalMilliseconds; }
            set { _retryIntervalMilliseconds = value; }
        }

        /// <summary>
        /// Whether to retry if a file has not been found or not.
        /// </summary>
        public bool RetryIfNotFound { get; set; }

        #endregion

        /// <summary>
        /// Scan remote directory for files matching the given file name pattern and download
        /// them, if any, to the given local directory.
        /// @see ITasklet#Execute
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            //Delegated 
            DoExecute();
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Delegated. Simplifies unit testing.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public bool DoExecute()
        {
            RemoveLocalFiles();
            // Compute remote files to be downloaded
            var remoteFiles = ComputeRemoteFiles();
            // now loop the downloads
            DownloadRemoteFiles(remoteFiles);
            return true;
        }

        /// <summary>
        /// Download list of remote files from remote directory
        /// </summary>
        /// <param name="remoteFiles"></param>
        /// <exception cref="Exception"></exception>
        private void DownloadRemoteFiles(IList<string> remoteFiles)
        {
            if (remoteFiles.Any())
            {
                foreach (var fileName in remoteFiles)
                {
                    // Start stopwatch
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Try download
                    var uri = string.Format("ftp://{0}:{1}/{2}/{3}", Host, Port, RemoteDirectory, fileName);
                    var request = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(Username, Password);
                    request.UseBinary = true;
                    request.UsePassive = false;

                    using (var response = (FtpWebResponse)request.GetResponse())
                    using (var inputStream = response.GetResponseStream())
                    using (var outputStream = File.OpenWrite(LocalDirectory + "/" + fileName))
                    {
                        inputStream.CopyTo(outputStream);
                    }

                    stopwatch.Stop();
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("File transfert of {0} done in {1} ms", fileName, stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }

        #region private utility methods
        /// <summary>
        /// Search for remote files matching the FileNamePattern
        /// </summary>
        /// <returns> a list of remote file names</returns>
        /// <exception cref="Exception"></exception>
        private IList<string> ComputeRemoteFiles()
        {
            IList<string> remoteFiles = new List<string>();
            var uri = string.Format("ftp://{0}:{1}/{2}", Host, Port, RemoteDirectory);
            var request = (FtpWebRequest)WebRequest.Create(new Uri(uri));
            request.Credentials = new NetworkCredential(Username, Password);

            var regex = RegexUtils.ConvertFilenameWildcardPatternToRegex(FileNamePattern);

            // List remote files to download
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            using (var response = (FtpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    //Do pattern matching here
                    if (regex.IsMatch(line))
                    {
                        remoteFiles.Add(line);
                    }
                    line = reader.ReadLine();
                }
            }
            return remoteFiles;
        }

        /// <summary>
        /// Remove local files prior to downloading fresh ones
        /// </summary>
        private void RemoveLocalFiles()
        {
            if (DeleteLocalFiles)
            {
                var matchingFiles = Directory.EnumerateFiles(LocalDirectory, FileNamePattern);
                var filePaths = matchingFiles as string[] ?? matchingFiles.ToArray();
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception e)
                        {
                            //catch anything and silently ignore on purpose
                            //add some debug info in case some issues need to be investigated
                            if (Logger.IsDebugEnabled)
                            {
                                Logger.Debug(e, "Exception encountered while trying to delete file ({0}) :{1}", filePath,
                                    e.Message);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Host, "Host attribute cannot be null");
            Assert.NotNull(Username, "Username attribute cannot be null");
            Assert.NotNull(Password, "Password attribute cannot be null");
            Assert.NotNull(LocalDirectory, "LocalDirectory attribute cannot be null");
            Assert.NotNull(RemoteDirectory, "RemoteDirectory attribute cannot be null");
            Assert.NotNull(FileNamePattern, "FileNamePattern attribute cannot be null");
            //Handle default values
            if (Port == null)
            {
                //Set default value
                Port = "21";
            }

            if (!Directory.Exists(LocalDirectory))
            {
                if (AutoCreateLocalDirectory)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug(string.Format("The {0} directory doesn't exist; Will create.", LocalDirectory));
                    }
                    Directory.CreateDirectory(LocalDirectory);
                }
                else
                {
                    throw new DirectoryNotFoundException(LocalDirectory);
                }
            }
        }
    }
}