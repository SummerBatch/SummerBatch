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
using NLog;
using Summer.Batch.Core;

namespace Summer.Batch.CoreTests.Batch.Listeners
{
    public class UppercaseListener : IStepExecutionListener
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void BeforeStep(StepExecution stepExecution)
        {
            _logger.Info("Starting uppercase test with seperate listener");
        }

        public ExitStatus AfterStep(StepExecution stepExecution)
        {
            _logger.Info("Ending uppercase test with seperate listener");
            return ExitStatus.Completed;
        }
    }
}