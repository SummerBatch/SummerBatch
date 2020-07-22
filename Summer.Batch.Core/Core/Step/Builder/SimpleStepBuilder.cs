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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using NLog;
using Summer.Batch.Core.Step.Item;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Infrastructure.Repeat.Policy;
using Summer.Batch.Infrastructure.Repeat.Support;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Step.Builder
{
    /// <summary>
    /// Step builder for step with reader, processor, and writer.
    /// </summary>
    public class SimpleStepBuilder : AbstractTaskletStepBuilder
    {
        private const string ChunkProviderSuffix = "__chunkProvider";
        private const string ChunkProcessorSuffix = "__chunkProcessor";
        private const int DefaultCommitInterval = 10;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _readerName;
        private string _processorName;
        private string _writerName;
        private int _chunkSize;
        private IRepeatOperations _chunkOperations;
        private ICompletionPolicy _completionPolicy;

        private readonly Type _inType;
        private readonly Type _outType;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="container">The container used to resolve the step.</param>
        /// <param name="name">The name of the step.</param>
        /// <param name="inType">The type of the read items.</param>
        /// <param name="outType">The type of the written items.</param>
        public SimpleStepBuilder(IUnityContainer container, string name, Type inType, Type outType)
            : base(container, name)
        {
            _inType = inType;
            _outType = outType;
        }

        public SimpleStepBuilder(IUnityContainer container, string name, Type inType, Type outType, int delayConfig)
            : base(container, name, delayConfig)
        {
            _inType = inType;
            _outType = outType;
        }

        /// <summary>
        /// Sets the reader.
        /// </summary>
        /// <param name="name">the name of the reader</param>
        /// <returns>the current step builder</returns>
        public SimpleStepBuilder Reader(string name)
        {
            _readerName = name;
            return this;
        }

        /// <summary>
        /// Sets the processor.
        /// </summary>
        /// <param name="name">the name of the processor</param>
        /// <returns>the current step builder</returns>
        public SimpleStepBuilder Processor(string name)
        {
            _processorName = name;
            return this;
        }

        /// <summary>
        /// Sets the writer.
        /// </summary>
        /// <param name="name">the name of the writer</param>
        /// <returns>the current step builder</returns>
        public SimpleStepBuilder Writer(string name)
        {
            _writerName = name;
            return this;
        }

        /// <summary>
        /// Sets the chunk size.
        /// </summary>
        /// <param name="size">the chunk size</param>
        /// <returns>the current step builder</returns>
        public SimpleStepBuilder ChunkSize(int size)
        {
            _chunkSize = size;
            return this;
        }

        /// <summary>
        /// Sets the chunk operations.
        /// </summary>
        /// <param name="chunkOperations">the chunk operations</param>
        /// <returns>the current step builder</returns>
        public SimpleStepBuilder ChunkOperations(IRepeatOperations chunkOperations)
        {
            _chunkOperations = chunkOperations;
            return this;
        }

        /// <summary>
        /// Sets the completion policy.
        /// </summary>
        /// <param name="completionPolicy">the completion policy</param>
        /// <returns>the current step builder</returns>
        public SimpleStepBuilder CompletionPolicy(ICompletionPolicy completionPolicy)
        {
            _completionPolicy = completionPolicy;
            return this;
        }

        /// <summary>
        /// Registers the tasklet with the given name.
        /// </summary>
        /// <returns>the name of the tasklet</returns>
        protected override string RegisterTasklet()
        {
            var taskletName = Name + TaskletSuffix;
            RegisterStreamsAndListeners();

            // Types
            var readerType = typeof(IItemReader<>).MakeGenericType(_inType);
            var processorType = typeof(IItemProcessor<,>).MakeGenericType(_inType, _outType);
            var writerType = typeof(IItemWriter<>).MakeGenericType(_outType);
            var chunkProviderType = typeof(SimpleChunkProvider<>).MakeGenericType(_inType);
            var chunkProcessorType = typeof(SimpleChunkProcessor<,>).MakeGenericType(_inType, _outType);
            var chunkTaskletType = typeof (ChunkOrientedTasklet<>).MakeGenericType(_inType);

            var chunkProviderName = taskletName + ChunkProviderSuffix;
            var chunkProcessorName = taskletName + ChunkProcessorSuffix;
            // Chunk provider registration
            Container.RegisterType(chunkProviderType, chunkProviderName,
                new InjectionConstructor(new ResolvedParameter(readerType, _readerName),
                                         GetChunkOperations()));
            // Chunk processor registration
            var processorInjection = _processorName == null
                ? (object)new InjectionParameter(processorType, null)
                : new ResolvedParameter(processorType, _processorName);
            Container.RegisterType(chunkProcessorType, chunkProcessorName,
                new InjectionConstructor(processorInjection,
                                         new ResolvedParameter(writerType, _writerName)));
            // Tasklet registration
            Container.RegisterType(typeof (ITasklet), chunkTaskletType, taskletName,
                new InjectionConstructor(new ResolvedParameter(chunkProviderType, chunkProviderName),
                                         new ResolvedParameter(chunkProcessorType, chunkProcessorName)));
            return taskletName;
        }

        /// <summary>
        /// Checks if the reader, the processor, or the writer are also streams or listeners.
        /// </summary>
        private void RegisterStreamsAndListeners()
        {
            // Types
            var readerType = typeof(IItemReader<>).MakeGenericType(_inType);
            var processorType = typeof(IItemProcessor<,>).MakeGenericType(_inType, _outType);
            var writerType = typeof(IItemWriter<>).MakeGenericType(_outType);
            
            var registrations = new Dictionary<string, ContainerRegistration>
            {
                { _readerName,
                  Container.Registrations.FirstOrDefault(r => r.RegisteredType == readerType
                      && r.Name == _readerName) },
                { _writerName,
                    Container.Registrations.FirstOrDefault(r => r.RegisteredType == writerType
                      && r.Name == _writerName) }
            };
            if (_processorName != null)
            {
                registrations[_processorName] =
                    Container.Registrations.FirstOrDefault(r => r.RegisteredType == processorType
                                                                && r.Name == _processorName);
            }
            foreach (var pair in registrations.Where(pair => pair.Value != null))
            {
                RegisterStreamAndListener(pair.Key, pair.Value.MappedToType);
            }
        }

        private void RegisterStreamAndListener(string name, Type type)
        {
            if (typeof(IItemStream).IsAssignableFrom(type))
            {
                Stream(type, name);
            }
            if (typeof(IStepExecutionListener).IsAssignableFrom(type))
            {
                Listener(type, name);
            }
        }

        private IRepeatOperations GetChunkOperations()
        {
            return _chunkOperations ?? new RepeatTemplate { CompletionPolicy = GetChunkCompletionPolicy() };
        }

        private ICompletionPolicy GetChunkCompletionPolicy()
        {
            Assert.State(!(_completionPolicy != null && _chunkSize > 0),
                "You must specify either a chunkCompletionPolicy or a commitInterval but not both.");
            Assert.State(_chunkSize >= 0, "The commitInterval must be positive or zero (for default value).");

            if (_completionPolicy != null)
            {
                return _completionPolicy;
            }
            if (_chunkSize == 0)
            {
                _logger.Info("Setting commit interval to default value (" + DefaultCommitInterval + ")");
                _chunkSize = DefaultCommitInterval;
            }
            return new SimpleCompletionPolicy(_chunkSize);
        }
    }
}