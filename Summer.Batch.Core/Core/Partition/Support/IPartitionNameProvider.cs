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

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// Optional interface for <see cref="IPartitioner"/> implementations that need to use a
    /// custom naming scheme for partitions. It is not necessary to implement this
    /// interface if a partitioner uses the default partition names.
    /// 
    /// If a partitioner does implement this interface, however, on a restart the
    /// IPartitioner#Partition(int) method will not be called again, instead
    /// the partitions will be re-used from the last execution, and matched by name
    /// with the results of IPartitionNameProvider#GetPartitionNames(int).
    /// This can be a useful performance optimisation if the partitioning process is
    /// expensive.
    /// </summary>
    public interface IPartitionNameProvider
    {
        /// <summary>
        /// </summary>
        /// <param name="gridSize"></param>
        /// <returns>the partition names given grid size</returns>
        ICollection<string> GetPartitionNames(int gridSize);
    }
}