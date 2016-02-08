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
using System.Text;
using System.Linq;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Item.Util;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Extra.IO
{
    /// <summary>
    /// A tasklet for basic file manipulations.
    /// 
    /// The operation is specified with the <see cref="Mode"/> property. Possible values are:
    /// 
    /// <list type="bullet">
    ///     <item>
    ///         <term>COPY</term>
    ///         <description>copies file to another location</description>
    ///     </item>
    ///     <item>
    ///         <term>DELETE</term>
    ///         <description>deletes a list of files</description>
    ///     </item>
    ///     <item>
    ///         <term>MERGE</term>
    ///         <description>appends the source files to the target file</description>
    ///     </item>
    ///     <item>
    ///         <term>MERGE_COPY</term>
    ///         <description>merges the source files to the target file (which is overwritten if it exists)</description>
    ///     </item>
    ///     <item>
    ///         <term>RESET</term>
    ///         <description>Creates new empty files, overwriting any existing file</description>
    ///     </item>
    /// </list>
    /// 
    /// </summary>
    public class FileUtilsTasklet : ITasklet, IInitializationPostOperations, IStepExecutionListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Enumeration of the possible file operations.
        /// </summary>
        public enum FileUtilsMode
        {
            /// <summary>
            /// Copy operation.
            /// </summary>
            Copy,
            /// <summary>
            /// Delete operation.
            /// </summary>
            Delete,
            /// <summary>
            /// Merge operation (sources files are appended to the target file).
            /// </summary>
            Merge,
            /// <summary>
            /// Merges the sources file and writes the result to the target file (which is overwritten).
            /// </summary>
            MergeCopy,
            /// <summary>
            /// Resets a file (the file is emptied).
            /// </summary>
            Reset,
            /// <summary>
            /// Compare 2 files for equality
            /// </summary>
            Compare
        };

        /// <summary>
        /// Enumeration of the possible file types.
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// Files to compare are text files
            /// </summary>
            Text,
            /// <summary>
            /// Files to compare are binary files, i.e. zip, etc...
            /// </summary>
            Binary
        }

        /// <summary>
        /// Enumeration of the possible Sequence EqualityComparer Types...
        /// </summary>
        public enum EqualityComparerType
        {
            /// <summary>
            /// Use Dafault IEqualityComparer...
            /// </summary>
            Default,
            /// <summary>
            /// Custom SequenceEqualityComparer similar to z/OS IEBCOMPR...
            /// </summary>
            IEBCOMPRLike
        }

        private EqualityComparerType _seqEqComparerType = EqualityComparerType.Default;
        /// <summary>
        /// FileType flag property.
        /// </summary>
        public EqualityComparerType SequenceEqualityComparerType
        {
            get { return _seqEqComparerType; }
            set { _seqEqComparerType = value; }
        }

        private FileType _fileType = FileType.Text;
        /// <summary>
        /// FileType flag property.
        /// </summary>
        public FileType FileCompareMode
        {
            get { return _fileType; }
            set { _fileType = value; }
        }

        private bool _strict = true;
        /// <summary>
        /// Strict mode flag property.
        /// </summary>
        public bool Strict
        {
            get { return _strict; }
            set { _strict = value; }
        }

        /// <summary>
        /// FileUtilsMode property.
        /// </summary>
        public FileUtilsMode Mode { private get; set; }

        /// <summary>
        /// List of source resources property.
        /// </summary>
        public IList<IResource> Sources { private get; set; }

        /// <summary>
        /// List of target resources property.
        /// </summary>
        public IList<IResource> Targets { private get; set; }

        private StepExecution _stepExecution;
        private JobInstance _jobInstance;
        /// <summary>
        /// Do nothing before step
        /// </summary>
        /// <param name="stepExecution"></param>
        public void BeforeStep(StepExecution stepExecution)
        {
            _stepExecution = stepExecution;
            _jobInstance = stepExecution.JobExecution.JobInstance;
        }

        //=> declare and set exit status...
        private ExitStatus _exitStatus = ExitStatus.Completed;

        public ExitStatus AfterStep(StepExecution stepExecution)
        {
            return _exitStatus;
        }


        /// <summary>
        /// @see ITasklet#Execute()
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            switch (Mode)
            {
                case FileUtilsMode.Copy:
                    Copy();
                    break;
                case FileUtilsMode.Delete:
                    foreach (IResource target in Targets)
                    {
                        if (target.Exists())
                        {
                            Delete(target);
                        }
                        else
                        {
                            Error(target);
                        }
                    }
                    break;
                case FileUtilsMode.Merge:
                    Merge(true);
                    break;
                case FileUtilsMode.MergeCopy:
                    Merge(false);
                    break;
                case FileUtilsMode.Reset:
                    Reset();
                    break;
                case FileUtilsMode.Compare:
                    this.Compare();
                    break;
                default:
                    throw new InvalidOperationException("This mode is not supported :[" + Mode + "]");
            }
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Do Reset
        /// </summary>
        private void Reset()
        {
            foreach (IResource target in Targets)
            {
                if (target.Exists())
                {
                    target.GetFileInfo().Open(FileMode.Truncate).Close();
                }
                else
                {
                    target.GetFileInfo().OpenWrite().Close();
                }
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Resetted file [{0}]", target.GetFullPath());
                }
            }
        }

        /// <summary>
        /// Do Delete
        /// </summary>
        /// <param name="target"></param>
        private static void Delete(IResource target)
        {
            //if Directory
            if (target.GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Deleting recursively folder [{0}]", target.GetFullPath());
                }
                Stopwatch sw = Stopwatch.StartNew();
                Directory.Delete(target.GetFullPath(), true);
                sw.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("[{0}] Folder deleted in {1} ms", target.GetFullPath(), sw.ElapsedMilliseconds);
                }
            }
            else // normal file
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Deleting file [{0}]", target.GetFullPath());
                }
                File.Delete(target.GetFullPath());
            }
        }

        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="append"></param>
        private void Merge(bool append)
        {
            FileMode fm = FileMode.Create;
            if (append)
            {
                fm = FileMode.Append;
            }
            bool sourceIsTarget = (Sources.Contains(Targets[0]));
            string tmpFileName = Path.GetTempFileName();

            // if source is target, use a temporary file that will be appended in the end. Otherwise, just use the given target.
            using (
                FileStream fs = sourceIsTarget
                    ? new FileStream(tmpFileName, FileMode.Create)
                    : Targets[0].GetFileInfo().Open(fm, FileAccess.Write))
            {
                foreach (var source in Sources)
                {
                    if (source.Exists())
                    {
                        using (Stream inStream = source.GetInputStream())
                        {
                            inStream.CopyTo(fs);
                        }
                    }
                    else
                    {
                        Error(source);
                    }
                }
            }
            if (sourceIsTarget)
            {
                //append tmpFile to existing target
                using (FileStream fsmerged = File.Open(Targets[0].GetFullPath(), fm, FileAccess.Write))
                using (Stream tmpStream = new FileInfo(tmpFileName).OpenRead())
                {
                    tmpStream.CopyTo(fsmerged);
                }
                File.Delete(tmpFileName);
            }
        }

        /// <summary>
        /// do copy
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Copy()
        {
            if (Sources.Count() == 1)
            {
                IResource source = Sources[0];
                if (source.GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (!Targets[0].GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
                    {
                        throw new Exception("Target must be a directory.");
                    }
                    else
                    {
                        //copy dir
                        FileUtils.CopyDir(source.GetFullPath(), Targets[0].GetFullPath());
                    }
                }
                else if (source.GetFileInfo().Attributes.HasFlag(FileAttributes.Normal))
                {
                    File.Copy(source.GetFullPath(), Targets[0].GetFullPath());
                }
                else
                {
                    Error(source);
                }
            }
            else
            {
                if (!Targets[0].GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
                {
                    throw new Exception("Target must be a directory.");
                }
                foreach (IResource source in Sources)
                {
                    if (source.Exists())
                    {
                        string targetName = Path.Combine(Targets[0].GetFileInfo().DirectoryName, source.GetFilename());
                        File.Copy(source.GetFullPath(), targetName);
                    }
                    else
                    {
                        Error(source);
                    }
                }
            }
        }

        /// <summary>
        /// Compare 2 files for equality
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Compare()
        {
            StringEqualityComparer strComparer = null;
            bool _filesEqual = true;

            if (this.FileCompareMode == FileType.Text)
            {
                //=> now compare text files...
                var f0Lines = File.ReadLines(Sources[0].GetFullPath());
                var f1Lines = File.ReadLines(Sources[1].GetFullPath());

                // The basic definition of two equal sequences is if they have:
                // * The same number of elements, and 
                // * The same values at each position in both sequences. 
                //
                // Note: SequenceEqual compares two collections for exact equality. 
                if (SequenceEqualityComparerType == EqualityComparerType.Default)
                {
                    //=> are files equal?
                    _filesEqual = f0Lines.SequenceEqual(f1Lines);
                }
                else if (SequenceEqualityComparerType == EqualityComparerType.IEBCOMPRLike)
                {
                    //=> our IEBCOMPRLike comparer records sequence index of first 9 lines that are different between 2 files...
                    strComparer = new StringEqualityComparer();

                    //=> Check seqComparer.SequenceEquality as seqComparer has a hack to test for upto 10 unequal records...
                    //   if # of unequal records is < 10 SequenceEqual will return true(which is not true, so we need to test SequenceEquality)
                    f0Lines.SequenceEqual(f1Lines, strComparer);

                    //=> are files equals?
                    _filesEqual = strComparer.SequenceEquality;

                    if (!_filesEqual && Logger.IsInfoEnabled)
                    {
                        StringBuilder sb = new StringBuilder();

                        //=> Build message...
                        sb.Append(Environment.NewLine);
                        sb.AppendFormat("*** JobName={0}, StepName={1}, Id={2} ***", _jobInstance.JobName, _stepExecution.StepName, _stepExecution.Id);
                        sb.Append(Environment.NewLine);
                        sb.Append("===> FILES ARE NOT EQUAL <===");
                        sb.Append(Environment.NewLine);

                        //=> lines from first file...
                        sb.AppendFormat("{0}", Sources[0].GetFilename());
                        sb.Append(Environment.NewLine);
                        foreach (int index in strComparer.seqNotEqIndexList)
                        {
                            var lNum = index + 1; //line numbers start with 1, List index is 0 based...
                            sb.AppendFormat("{0,8:D8}: {1}", lNum, f0Lines.ElementAt(index).ToString());
                            sb.Append(Environment.NewLine);
                        }

                        //=> lines from second file...
                        sb.AppendFormat("{0}", Sources[1].GetFilename());
                        sb.Append(Environment.NewLine);
                        foreach (int index in strComparer.seqNotEqIndexList)
                        {
                            var lNum = index + 1; //line numbers start with 1, List index is 0 based...
                            sb.AppendFormat("{0,8:D8}: {1}", lNum, f1Lines.ElementAt(index).ToString());
                            sb.Append(Environment.NewLine);
                        }

                        //=> log it...
                        Logger.Info(sb.ToString());
                    }
                }
            }
            else if (this.FileCompareMode == FileType.Binary)
            {
                //=> are files the same?
                _filesEqual = FileCompare(Sources[0].GetFileInfo(), Sources[1].GetFileInfo());
            }

            //=> if files equal...
            if (_filesEqual)
            {
                Logger.Info("===> FILES {0} and {1} ARE EQUAL <===", Sources[0].GetFilename(), Sources[1].GetFilename());
            }
            else
            {
                _exitStatus = new ExitStatus("FILESNOTEQUAL", "FILES ARE NOT EQUAL.");

                if (Logger.IsInfoEnabled && SequenceEqualityComparerType != EqualityComparerType.IEBCOMPRLike)
                    Logger.Info("===> FILES {0} and {1} ARE NOT EQUAL <===", Sources[0].GetFilename(), Sources[1].GetFilename());
            }
        }

        //=> code for this method was taken from https://support.microsoft.com/en-us/kb/320348
        // This method accepts two FileInfo parameters that represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private bool FileCompare(FileInfo file1, FileInfo file2)
        {
            int file1byte;
            int file2byte;

            // Determine if the same file was referenced two times.
            if (file1.FullName == file2.FullName)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Check the file sizes. If they are not the same, the files are not the same.
            if (file1.Length != file2.Length)
                return false; // Return false to indicate files are different

            // Open the two files.
            using (FileStream fs1 = new FileStream(file1.FullName, FileMode.Open))
            using (FileStream fs2 = new FileStream(file2.FullName, FileMode.Open))
            {
                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                do
                {
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));
            }

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        /// <summary>
        /// Check proper properties initialization, depending on selected mode.
        /// @see IInitializationPostOperations#AfterPropertiesSet()
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Mode, "Mode should be specified");
            switch (Mode)
            {
                case FileUtilsMode.Copy:
                case FileUtilsMode.Merge:
                case FileUtilsMode.MergeCopy:
                    Assert.State(Targets != null && Targets.Count == 1, "one and only one target must be specified.");
                    if (Strict)
                    {
                        Assert.NotEmpty(Sources, "STRICT MODE : at least one source must be specified.");
                    }
                    else if (!Sources.Any())
                    {
                        Logger.Warn("no sources specified.");
                    }
                    break;
                case FileUtilsMode.Delete:
                case FileUtilsMode.Reset:
                    if (Strict)
                    {
                        Assert.NotEmpty(Targets, "STRICT MODE : at least one target must be specified.");
                    }
                    else if (!Targets.Any())
                    {
                        Logger.Warn("no targets specified.");
                    }
                    break;
                case FileUtilsMode.Compare:
                    //=> we need at least 2 file resources to compare...
                    Assert.State(Sources != null && Sources.Count == 2, "Files Compare operation requires that 2 files must be specified.");

                    //=> now lets make sure resources exist and are files...
                    foreach (IResource source in Sources)
                    {
                        //=> make sure source exists...
                        Assert.State(source.Exists(), source.GetDescription() + " does not exist.");

                        //=> source should be a file (NOT Directory)...
                        Assert.State(!source.GetFileInfo().Attributes.HasFlag(FileAttributes.Directory), source.GetDescription() + " shoul be a file, NOT Directory.");
                    }
                    break;
                default:
                    throw new InvalidOperationException("This mode is not supported :[" + Mode + "]");
            }
        }

        /// <summary>
        /// log error or throw exception, depending on Strict attribute value
        /// </summary>
        /// <param name="resource"></param>
        private void Error(IResource resource)
        {
            string message = resource.GetDescription() + " does not exist.";
            if (Strict)
            {
                throw new Exception(message);
            }
            else
            {
                Logger.Warn(message);
            }
        }
    }

    //=> string comparer for Enumerable.SequenceEqual...
    //   we created this comparer to trap index location of the first index not equal between the 2 files...
    internal class StringEqualityComparer : IEqualityComparer<string>
    {
        private int _countNotEqLines = 0;
        private int seqCount = 0;
        internal protected List<int> seqNotEqIndexList = new List<int>();
        internal protected bool SequenceEquality = true;

        public bool Equals(string s1, string s2)
        {
            //=> compare strings...
            bool s1EQs2 = s1.Equals(s2);

            // NOTE: Enumerable.SequenceEqual will stop comparison after first NOT equal compare in the sequence...
            //       we want equivalence with z/OS IEBCOMPR, i.e. by default 10 successive unequal comparisons will stop the IEBCOMPR  
            if (!s1EQs2)
            {
                //=> return false if count of unequal lines is 10... 
                if (++_countNotEqLines > 9)
                    return false;

                //=> get index of unequal records and increment count...
                seqNotEqIndexList.Add(seqCount++);

                //=> hack to know if SequenceEqual should return false BUT returns true since # of unequal lines _countNotEqLines < 10
                //   make sure to test this parameter when Enumerable.SequenceEqual returns...
                SequenceEquality = false;

                //=> return true so we can continue till _countNotEqLines count reaches 10...
                //   if _countNotEqLines does not reach 10, Enumerable.SequenceEqual return of true is wrong
                //   make sure to test SequenceEquality for the right value!!!
                return true;
            }

            //=> keep track of index sequence count...
            seqCount++;

            return s1EQs2;
        }

        public int GetHashCode(string s)
        {
            return s.GetHashCode();
        }
    }

}