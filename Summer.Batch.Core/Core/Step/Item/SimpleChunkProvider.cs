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
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Repeat;
using System;

namespace Summer.Batch.Core.Step.Item
{

    /// <summary>
    /// Simple implementation of the <see cref="T:IChunkProvider"/> interface that 
    /// does basic chunk providing from an <see cref="T:Summer.Batch.Infrastructure.Item.IItemReader"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleChunkProvider<T> : IChunkProvider<T> where T : class
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Item reader property.
        /// </summary>
        public IItemReader<T> ItemReader { get; protected set; }
        private readonly IRepeatOperations _repeatOperations;

        /// <summary>
        /// Custom constructor
        /// </summary>
        /// <param name="itemReader"></param>
        /// <param name="repeatOperations"></param>
        public SimpleChunkProvider(IItemReader<T> itemReader, IRepeatOperations repeatOperations)
        {
            ItemReader = itemReader;
            _repeatOperations = repeatOperations;
        }

        /// <summary>
        /// Read
        /// Delegates to #DoRead(). Subclasses can add additional behaviour
        /// (e.g. exception handling).
        /// </summary>
        /// <param name="contribution">the current step execution contribution</param>
        /// <param name="chunk">the current chunk</param>
        /// <returns>a new item for processing</returns>
        /// <exception cref="Exception">if there is a generic issue</exception>
        protected T Read(StepContribution contribution, Chunk<T> chunk)
        {
            return DoRead();
        }

        /// <summary>
        /// Read delegation.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected T DoRead()
        {
            try
            {
                T item = ItemReader.Read();
                return item;
            }
            catch (Exception e)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug(e.Message + " : " + e.GetType().Name);
                }
                throw;
            }
        }

        /// <summary>
        /// @see IChunkProvider#Provide.
        /// </summary>
        /// <param name="contribution"></param>
        /// <returns></returns>
        public Chunk<T> Provide(StepContribution contribution)
        {
            Chunk<T> inputs = new Chunk<T>();
            _repeatOperations.Iterate(context =>
            {
                var item = Read(contribution, inputs);
                if (item == null)
                {
                    inputs.End = true;
                    return RepeatStatus.Finished;
                }
                inputs.Add(item);
                contribution.IncrementReadCount();
                return RepeatStatus.Continuable;
            });
            return inputs;
        }


        /// <summary>
        /// @see IChunkProvider#PostProcess.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunk"></param>
        public void PostProcess(StepContribution contribution, Chunk<T> chunk)
        {
            //Left empty on purpose (do nothing)
        }
    }
}