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

using Summer.Batch.Infrastructure.Item;
using System;
using System.Collections.Generic;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Step.Item
{

    /// <summary>
    /// Simple implementation of the <see cref="T:IChunkProcessor"/>interface that handles
    /// basic item writing and processing. Any exceptions encountered will be
    /// rethrown.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public class SimpleChunkProcessor<TIn, TOut> : IChunkProcessor<TIn>, IInitializationPostOperations
        where TIn : class
        where TOut : class
    {
        /// <summary>
        /// Item processor property.
        /// </summary>
        public IItemProcessor<TIn, TOut> ItemProcessor { get; set; }

        /// <summary>
        /// Item writer property.
        /// </summary>
        public IItemWriter<TOut> ItemWriter { get; set; }

        #region public methods
        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(ItemWriter, "ItemWriter must be set");
        }

        /// <summary>
        /// Custom constructor using and itemprocessor and an itemwriter.
        /// </summary>
        /// <param name="itemProcessor"></param>
        /// <param name="itemWriter"></param>
        public SimpleChunkProcessor(IItemProcessor<TIn, TOut> itemProcessor, IItemWriter<TOut> itemWriter)
        {
            ItemProcessor = itemProcessor;
            ItemWriter = itemWriter;
        }

        /// <summary>
        /// see IChunkProcessor#Process.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="inputs"></param>
        public void Process(StepContribution contribution, Chunk<TIn> inputs)
        {
            InitializeUserData(inputs);
            if (IsComplete(inputs))
            {
                return;
            }
            Chunk<TOut> outputs = Transform(contribution, inputs);

            contribution.IncrementFilterCount(GetFilterCount(inputs, outputs));
            Write(contribution, inputs, GetAdjustedOutputs(inputs, outputs));
        } 
        #endregion

        #region protected methods
        /// <summary>
        ///  Extension point for subclasses to allow them to memorise the contents of
        /// the inputs, in case they are needed for accounting purposes later. The
        /// default implementation sets up some user data to remember the original
        /// size of the inputs. If this method is overridden then some or all of
        /// #IsComplete(Chunk), #GetFilterCount(Chunk, Chunk)} and
        /// #GetAdjustedOutputs(Chunk, Chunk)} might also need to be, to
        /// ensure that the user data is handled consistently.
        /// </summary>
        /// <param name="inputs">the inputs for the process</param>
        protected void InitializeUserData(Chunk<TIn> inputs)
        {
            inputs.UserData = inputs.Size();
        }

        /// <summary>
        /// Extension point for subclasses that want to store additional data in the
        /// inputs. Default just checks if inputs are empty.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        protected bool IsComplete(Chunk<TIn> inputs)
        {
            return inputs.IsEmpty();
        }

        /// <summary>
        /// Process item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        protected TOut DoProcess(TIn item)
        {
            if (ItemProcessor == null)
            {
                object result = item;
                return (TOut)result;
            }
            object resultProcess = ItemProcessor.Process(item);
            return (TOut)resultProcess;
        }


        /// <summary>
        /// Transform inputs.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        protected Chunk<TOut> Transform(StepContribution contribution, Chunk<TIn> inputs)
        {
            var outputs = new Chunk<TOut>();
            var toRemove = new List<TIn>();
            foreach (var item in inputs.Items)
            {
                TOut output;
                try
                {
                    output = DoProcess(item);
                }
                catch (Exception)
                {
                    // For a simple chunk processor (no fault tolerance) we are done
                    // here, so prevent any more processing of these inputs.				 
                    inputs.Clear();
                    throw;
                }

                if (output != null)
                {
                    outputs.Add(output);
                }
                else
                {
                    toRemove.Add(item);
                }
            }
            toRemove.ForEach(i => inputs.Items.Remove(i));
            return outputs;
        }


        /// <summary>
        ///  Extension point for subclasses to calculate the filter count. Defaults to
        /// the difference between input size and output size.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        protected int GetFilterCount(Chunk<TIn> inputs, Chunk<TOut> outputs)
        {
            return ((int)inputs.UserData) - outputs.Size();
        }

        /// <summary>
        /// Extension point for subclasses that want to adjust the outputs based on
        /// additional saved data in the inputs. Default implementation just returns
        /// the outputs unchanged.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        protected Chunk<TOut> GetAdjustedOutputs(Chunk<TIn> inputs, Chunk<TOut> outputs)
        {
            return outputs;
        }

        /// <summary>
        /// Simple implementation delegates to the #DoWrite(List) method and
        /// increments the write count in the contribution. Subclasses can handle
        /// more complicated scenarios, e.g.with fault tolerance. If output items are
        /// skipped they should be removed from the inputs as well.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <exception cref="Exception"></exception>
        protected void Write(StepContribution contribution, Chunk<TIn> inputs, Chunk<TOut> outputs)
        {
            try
            {
                DoWrite(outputs.Items);
            }
            catch (Exception)
            {                
                //For a simple chunk processor (no fault tolerance) we are done
                // here, so prevent any more processing of these inputs.
                inputs.Clear();
                throw;
            }
            contribution.IncrementWriteCount(outputs.Size());
        }

        /// <summary>
        /// Actual write operation. Delegates to WriteItems.
        /// </summary>
        /// <param name="items"></param>
        /// <exception cref="Exception"></exception>
        protected void DoWrite(IList<TOut> items)
        {
            if (ItemWriter == null)
            {
                return;
            }
            WriteItems(items);
        }

        /// <summary>
        /// Write list of items
        /// </summary>
        /// <param name="items"></param>
        protected void WriteItems(IList<TOut> items)
        {
            if (ItemWriter != null)
            {
                ItemWriter.Write(items);
            }
        } 
        #endregion

    }
}