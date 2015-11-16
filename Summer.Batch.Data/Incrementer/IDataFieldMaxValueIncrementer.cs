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

using System.Configuration;

namespace Summer.Batch.Data.Incrementer
{
    /// <summary>
    /// Interface that defines contract of incrementing any data store field's maximum value. Works much like a sequence number generator.
    /// Typical implementations may use standard SQL, native RDBMS sequences or Stored Procedures to do the job.
    /// </summary>
    public interface IDataFieldMaxValueIncrementer
    {
        /// <summary>
        /// The connection string settings to use for connecting the database.
        /// </summary>
        ConnectionStringSettings ConnectionStringSettings { set; }

        /// <summary>
        /// The name of the incrementer in the database.
        /// </summary>
        string IncrementerName { get; set; }

        /// <summary>
        /// Increment the data store field's max value.
        /// </summary>
        /// <returns>the new value</returns>
        long NextLong();
    }
}
