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

using System;

namespace Summer.Batch.Core.Step.Item
{
    /// <summary>
    /// Interface for providing Chunks to be processed, used by the <see cref="T:ChunkOrientedTasklet"/>.
    /// </summary>
    /// <typeparam name="TT">&nbsp;</typeparam>
    public interface IChunkProvider<TT>
    {
        /// <summary>
        /// Provides the chunk.
        /// </summary>
        /// <param name="contribution"></param>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        Chunk<TT> Provide(StepContribution contribution); 
    
        /// <summary>
        /// Post-processing operation support.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunk"></param>
        void PostProcess(StepContribution contribution, Chunk<TT> chunk);
    }
}