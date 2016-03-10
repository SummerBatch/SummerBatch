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

using NLog;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Core.Step.Item
{
    /// <summary>
    ///  An <see cref="ITasklet"/> implementing variations on read-process-write item handling.
    /// </summary>
    /// <typeparam name="TI">&nbsp;</typeparam>
    public class ChunkOrientedTasklet<TI> : ITasklet where TI:class
    {
        private const string InputsKey = "INPUTS";

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Attributes

        private bool _buffering = true;
        private readonly IChunkProvider<TI> _chunkProvider;
        private readonly IChunkProcessor<TI> _chunkProcessor;

        #endregion

        /// <summary>
        /// Flag to indicate that items should be buffered once read. Defaults to
        /// true, which is appropriate for forward-only, non-transactional item
        /// readers. Main (or only) use case for setting this flag to false is a
        /// transactional JMS item reader
        /// </summary>
        public bool Buffering { set { _buffering = value; } }

        /// <summary>
        /// Chunk provider property.
        /// </summary>
        public IChunkProvider<TI> ChunkProvider { get { return _chunkProvider; } }

        /// <summary>
        /// Chunk processor property.
        /// </summary>
        public IChunkProcessor<TI> ChunkProcessor { get { return _chunkProcessor; } }

        /// <summary>
        /// Custom constructor using a chunk provider and a chunk processor.
        /// </summary>
        /// <param name="chunkProvider"></param>
        /// <param name="chunkProcessor"></param>
        public ChunkOrientedTasklet(IChunkProvider<TI> chunkProvider, IChunkProcessor<TI> chunkProcessor)
        {
            _chunkProvider = chunkProvider;
            _chunkProcessor = chunkProcessor;
        }

        /// <summary>
        /// @see ITasklet#Execute.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            Chunk<TI> inputs = (Chunk<TI>)chunkContext.GetAttribute(InputsKey);
            if (inputs == null)
            {
                inputs = _chunkProvider.Provide(contribution);
                if (_buffering)
                {
                    chunkContext.SetAttribute(InputsKey, inputs);
                }
            }

            _chunkProcessor.Process(contribution, inputs);
            _chunkProvider.PostProcess(contribution, inputs);

            // Allow a message coming back from the processor to say that we
            // are not done yet
            if (inputs.Busy)
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("Inputs still busy");
                }
                return RepeatStatus.Continuable;
            }

            chunkContext.RemoveAttribute(InputsKey);
            chunkContext.SetComplete();
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Inputs not busy, ended: {0}" , inputs.End);
            }
            return RepeatStatus.ContinueIf(!inputs.End);
        }
    }
}