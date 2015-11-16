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
using System.Diagnostics;
using System.IO;
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
    /// Custom tasklet to do some ftp put
    /// Sftp is unsupported for now
    /// </summary>
    public class FtpPutTasklet : ITasklet, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// File name property.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Ftp server host property.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Ftp server port property.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Ftp remote directory property.
        /// </summary>
        public string RemoteDirectory { get; set; }
        
        /// <summary>
        /// Ftp server user name property.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Ftp server user password property.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// @see ITasklet#Execute
        /// Upload the given file to remote directory
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
        /// Delegate. Simplifies unit testing.
        /// </summary>
        /// <returns></returns>
        public bool DoExecute()
        {
            var fileInfo = new FileInfo(FileName);
            if (fileInfo.Exists)
            {
                // Start stopwatch
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // Get the object used to communicate with the server.
                var uri = string.Format("ftp://{0}:{1}/{2}/{3}", Host, Port, RemoteDirectory, fileInfo.Name);
                var request = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UsePassive = false;
                request.UseBinary = true;

                // Set needed credentials
                request.Credentials = new NetworkCredential(Username, Password);

                // Copy the contents of the file to the request stream.
                request.ContentLength = fileInfo.Length;
                using (var inputStream = fileInfo.OpenRead())
                using (var requestStream = request.GetRequestStream())
                {
                    inputStream.CopyTo(requestStream);
                }

                using (var response = (FtpWebResponse) request.GetResponse())
                {
                    stopwatch.Stop();
                    Logger.Info("Upload File Complete, status {0} in {1} ms", response.StatusDescription,
                        stopwatch.ElapsedMilliseconds);
                }
            }
            else
            {
                Logger.Error("File " + FileName + " not found.");
            }
            return true;
        }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(FileName, "FileName attribute cannot be null");
            Assert.NotNull(Host, "Host attribute cannot be null");
            Assert.NotNull(Username, "Username attribute cannot be null");
            Assert.NotNull(Password, "Password attribute cannot be null");
            //Handle default values
            if (Port == null)
            {
                //Set default value
                Port = "21";
            }
            if (RemoteDirectory == null)
            {
                RemoteDirectory = string.Empty;
            }
        }
    }
}