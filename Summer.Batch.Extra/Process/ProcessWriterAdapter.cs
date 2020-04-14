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
using System.Collections.Generic;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Process
{
    /// <summary>
    /// This class allows an IItemWriter to be used in a process.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class ProcessWriterAdapter<T> : AbstractProcessAdapter<T>, IProcessWriter<T> where T : class
    {
        /// <summary>
        /// The underlying IItemWriter 
        /// </summary>
        private IItemWriter<T> _writer;

        /// <summary>
        /// The adaptee writer.
        /// </summary>
        public IItemWriter<T> Adaptee
        {
            set
            {
                _writer = value;
                RegisterStream(value);
            }
        }

        /// <summary>
        /// Writes a record
        /// </summary>
        /// <param name="obj">the record</param>
        public void WriteInProcess(T obj)
        {
            InitStream();
            _writer.Write(new List<T> {obj});
            UpdateStream();
        }
    }
}