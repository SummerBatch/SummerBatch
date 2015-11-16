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
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Process
{
    /// <summary>
    ///  This class allows an IItemReader to be used in a process.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProcessReaderAdapter<T> : AbstractProcessAdapter<T>, IProcessReader<T> where T : class
    {
        /// <summary>
        /// the underlying stream
        /// </summary>
        private IItemReader<T> _reader;

        /// <summary>
        /// The adaptee reader.
        /// </summary>
        public IItemReader<T> Adaptee
        {
            set
            {
                _reader = value;
                RegisterStream(value);
            }
        }

        /// <summary>
        /// Reads a record from the underlying reader
        /// </summary>
        /// <returns>the object mapping read data</returns>
        public T ReadInProcess()
        {
            // First, open the stream (if there is a stream to open and it was not already done).
            InitStream();
            //then, read
            return  _reader.Read();
        }
    }
}