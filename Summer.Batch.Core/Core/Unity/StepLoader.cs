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
using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Summer.Batch.Core.Partition.Support;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Step.Builder;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Loads a step from an XML definition and a Unity _container.
    /// </summary>
    public class StepLoader
    {
        private readonly XmlStep _step;
        private readonly IUnityContainer _container;

        /// <summary>
        /// Constructs a new <see cref="StepLoader"/> with the specified input and output types.
        /// </summary>
        public StepLoader(XmlStep step, IUnityContainer container)
        {
            _step = step;
            _container = container;
        }

        /// <summary>
        /// The step loading, using xml definition, the unity _container and the parameter signature.
        /// </summary>
        /// <returns></returns>
        public IStep LoadStep()
        {
            if (_step.Partition != null && _step.Partition.Mapper != null)
            {
                return new PartitionStepBuilder(_container, _step.Id + "Partition")
                    .Partitioner(_step.Id, _container.Resolve<IPartitioner>(_step.Partition.Mapper.Ref))
                    .GridSize(_step.Partition.Mapper.GridSize)
                    .Step(DoLoadStep())
                    .TaskExecutor(_container.Resolve<ITaskExecutor>())
                    .Repository(_container.Resolve<IJobRepository>())
                    .Build();
            }
            return DoLoadStep();
        }

        private IStep DoLoadStep()
        {
            if (_step.Batchlet != null)
            {
                var builder = new TaskletStepBuilder(_container, _step.Id).Tasklet(_step.Batchlet.Ref);
                builder.Repository(_container.Resolve<IJobRepository>());
                AddListeners(builder);
                return builder.Build();
            }
            if (_step.Chunk != null)
            {
                var inType = GetTypeParameter(_container, typeof(IItemReader<>), _step.Chunk.Reader.Ref);
                var outType = GetTypeParameter(_container, typeof(IItemWriter<>), _step.Chunk.Writer.Ref);
                var builder = new SimpleStepBuilder(_container, _step.Id, inType, outType)
                    .Reader(_step.Chunk.Reader.Ref)
                    .Writer(_step.Chunk.Writer.Ref);
                if (_step.Chunk.Processor != null && !string.IsNullOrEmpty(_step.Chunk.Processor.Ref))
                {
                    builder.Processor(_step.Chunk.Processor.Ref);
                }
                if (!string.IsNullOrEmpty(_step.Chunk.ItemCount))
                {
                    builder.ChunkSize(int.Parse(_step.Chunk.ItemCount));
                }
                builder.Repository(_container.Resolve<IJobRepository>());
                AddListeners(builder);
                return builder.Build();
            }
            throw new ArgumentException("A Batchlet or a chunk must be provided in the step");
        }

        // Adds the listeners of the step to the builder.
        private void AddListeners(AbstractStepBuilder builder)
        {
            if (_step.Listeners != null && _step.Listeners.Listeners != null)
            {
                foreach (var listener in _step.Listeners.Listeners)
                {
                    builder.Listener(typeof(IStepExecutionListener), listener.Ref);
                }
            }
        }

        // Searches for a registration with the given name registered with the given generic interface
        // and returns the generic parameter.
        private static Type GetTypeParameter(IUnityContainer container, Type genericInterface, string name)
        {
            var registrations = container.Registrations.Where(r => r.Name == name).ToArray();
            if (registrations.Length != 1)
            {
                throw new RegistrationException(
                    string.Format("The container must have one and only registration with name \"{0}\" and type [{1}]",
                        name, genericInterface.FullName));
            }
            var registeredType = registrations[0].RegisteredType;
            if (registeredType.GetGenericTypeDefinition() != genericInterface)
            {
                throw new RegistrationException(string.Format(
                    "Registration with name \"{0}\" was expected with type [{1}].", name, genericInterface.FullName));
            }
            return registeredType.GenericTypeArguments[0];
        }
    }
}