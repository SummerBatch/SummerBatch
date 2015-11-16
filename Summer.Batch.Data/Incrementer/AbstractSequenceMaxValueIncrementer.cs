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

namespace Summer.Batch.Data.Incrementer
{
    /// <summary>
    /// Abstract class for sequence maximum value incrementer.
    /// </summary>
    public abstract class AbstractSequenceMaxValueIncrementer : AbstractDataFieldMaxValueIncrementer
    {
        /// <summary>
        /// Returns the next value for the set sequence.
        /// </summary>
        /// <returns></returns>
        public override long NextLong()
        {
            using (var connection = GetConnection())
            {
                using (var insertCommand = GetCommand(GetSequenceQuery(), connection))
                {
                    return Converter.Convert<long>(insertCommand.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Return the database-specific query to use for retrieving a sequence value.
        /// <p>The provided SQL is supposed to result in a single row with a single
        /// column that allows for extracting a long value.</p>
        /// </summary>
        protected abstract string GetSequenceQuery();

    }
}