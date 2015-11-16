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
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.CoreTests.Batch.Listeners
{
    public class OkTasklet : ITasklet
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            _logger.Info("OK transition was used");
            return RepeatStatus.Finished;
        }
    }
}