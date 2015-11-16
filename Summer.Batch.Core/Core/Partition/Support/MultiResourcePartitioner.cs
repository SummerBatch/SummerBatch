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

using System.Collections.Generic;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// Implementation of <see cref="IPartitioner"/> that locates multiple resources and associates their absolute
    /// URIs in the execution contexts. Create one execution context per resource, whatever the grid size.
    /// </summary>
    public class MultiResourcePartitioner : IPartitioner
    {
        private const string DefaultKeyName = "fileName";
        private const string DefaultPartitionIdName = "partitionId";
        private const string PartitionKey = "partition";

        /// <summary>
        /// The resources to assign to each partition.
        /// </summary>
        public IList<IResource> Resources { get; set; }

        /// <summary>
        /// The key used to store the fileName in the context. Default is DefaultKeyName.
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// The key used to store the id of the partition. Default is DefaultPartitionIdName.
        /// </summary>
        public string PartitionIdName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MultiResourcePartitioner()
        {
            KeyName = DefaultKeyName;
            PartitionIdName = DefaultPartitionIdName;
        }

        /// <summary>
        /// Creates one partition per resource and adds the URI in the context.
        /// </summary>
        /// <param name="gridSize">ignored</param>
        /// <returns>a dictionary containing an execution context per resource</returns>
        public IDictionary<string, ExecutionContext> Partition(int gridSize)
        {
            var contexts = new Dictionary<string, ExecutionContext>();
            for (var i = 0; i < Resources.Count; i++)
            {
                var resource = Resources[i];
                var context = new ExecutionContext();
                Assert.State(resource.Exists(), string.Format("Resource does not exist: {0}",resource));
                context.PutString(KeyName, resource.GetUri().AbsoluteUri);
                context.PutInt(PartitionIdName, i);
                contexts[PartitionKey + i] = context;
            }
            return contexts;
        }
    }
}