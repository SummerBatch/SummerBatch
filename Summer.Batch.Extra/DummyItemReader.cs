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

using NLog;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra
{
    /// <summary>
    /// Dummy reader that will always return a unique result (<see cref="DefaultResult"/>).
    /// </summary>
    public class DummyItemReader<T> : IItemReader<object>
    {
        private const string DefaultResult = "NULL";

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inner flag to do only one dummy read.
        /// </summary>
        private bool _hasAlreadyDoneOneRead;

        /// <summary>
        /// Reader that will only return one item.
        /// </summary>
        /// <returns><see cref="DefaultResult"/> the first time, then null.</returns>
        public object Read()
        {
            var result = _hasAlreadyDoneOneRead ? null : DefaultResult;

            _logger.Trace("DummyItemReader ({0}) - _hasAlreadyDoneOneRead={1} - returning {2}",
                typeof(T).FullName, _hasAlreadyDoneOneRead, result);

            _hasAlreadyDoneOneRead = true;
            return result;
        }
    }
}