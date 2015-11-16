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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using NLog;

namespace Summer.Batch.Extra.Service.Stop
{
    /// <summary>
    /// Service use to sto stop a job or skipping a record.
    /// </summary>
    public class ServiceStop
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Stops the current job.
        /// </summary>
        /// <param name="message">the error message</param>
        public void StopJob(string message)
        {
            Logger.Error("Job stopped: " + message);
            throw new FailedJobException(message);
        }

        /// <summary>
        /// Skips the current record.
        /// </summary>
        /// <param name="message">the message describing the reason for skipping the item</param>
        public void SkipRecord(string message)
        {
            Logger.Info("Item skipped: " + message);
            throw new SkippedItemException(message);
        }
    }
}