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

//   This file has been modified.
//   Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Infrastructure.Repeat;
using System;

namespace Summer.Batch.Core.Step.Tasklet
{
    /// <summary>
    /// Strategy for processing in a step.
    /// </summary>
    public interface ITasklet
    {
        /// <summary>
        /// Given the current context in the form of a step contribution, do whatever
        /// is necessary to process this unit inside a transaction. Implementations
        /// return RepeatStatus#Finished if finished. If not they return
        /// RepeatStatus#Continuable. On failure throws an exception.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext);

    }
}