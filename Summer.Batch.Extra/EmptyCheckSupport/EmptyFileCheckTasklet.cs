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
using System.IO;
using NLog;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Extra.EmptyCheckSupport
{
    /// <summary>
    /// Simple tasklet that checks if a given file is empty. It returns "EMPTY"
    /// if the file is empty or absent, "NOT_EMPTY" if it exists and is not empty.
    /// </summary>
    public class EmptyFileCheckTasklet : ITasklet, IStepExecutionListener
    {
        private const string Empty = "EMPTY";
        private const string NotEmpty = "NOT_EMPTY";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The file to check
        /// </summary>
        public IResource FileToCheck { private get; set; }

        /// <summary>
        /// Do nothing before step
        /// </summary>
        /// <param name="stepExecution"></param>
        public void BeforeStep(StepExecution stepExecution)
        {
            // Do nothing
        }

        /// <summary>
        /// Do nothing execution, since all the logic is in after step
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Performs the actual check and returns the code "EMPTY" or "NOT_EMPTY" accordingly
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns>"EMPTY" or "NOT_EMPTY"</returns>
        public ExitStatus AfterStep(StepExecution stepExecution)
        {
            string exitCode = Empty;

            try
            {
                FileInfo file = FileToCheck.GetFileInfo();

                if (file.Exists && (file.Attributes & FileAttributes.Directory) != FileAttributes.Directory &&
                    file.Length > 0)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("File [{0}] is not empty ! length =[{1}]", file.FullName, file.Length);
                    }
                    exitCode = NotEmpty;
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("File [{0}] is empty .", file.FullName);
                    }
                }
            }
            catch (IOException)
            {
                Logger.Error("Error accessing file " + FileToCheck.GetFilename());
            }
           
            return new ExitStatus(exitCode);
        }
    }
}