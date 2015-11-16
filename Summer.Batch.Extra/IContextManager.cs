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

namespace Summer.Batch.Extra
{
    /// <summary>
    /// Interface for context manager
    /// </summary>
    public interface IContextManager
    {
        /// <summary>
        /// Accessors for the context
        /// </summary>
        ExecutionContext Context { get; set; }

        /// <summary>
        /// Stores an object inside the cache
        /// </summary>
        /// <param name="key">object key to store</param>
        /// <param name="record">object to store</param>
        void PutInContext(object key, object record);

        /// <summary>
        /// Check if the key is in the cache
        /// </summary>
        /// <param name="key">key of the object to retrieve</param>
        /// <returns>whether it is in the cache</returns>
        bool ContainsKey(object key);

        /// <summary>
        /// Retrieves an object from the cache
        /// </summary>
        /// <param name="key">key of the object to retrieve</param>
        /// <returns>retrieved object</returns>
        object GetFromContext(object key);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Empty();

        /// <summary>
        /// Dumps the cache content
        /// </summary>
        /// <returns>the content of the cache as a string</returns>
        string Dump();

        /// <summary>
        /// Sets the value of a named counter
        /// </summary>
        /// <param name="counter">the name of the counter</param>
        /// <param name="value">the new value of the the named counter</param>
        void SetCounter(string counter, long value);

        /// <summary>
        /// Returns the value of a named counter
        /// </summary>
        /// <param name="counter">the name of the counter</param>
        ///<returns>the value of the the named counter</returns>
        long GetCounter(string counter);

        /// <summary>
        ///  Increments the value of a counter by one. If this counter does not yet
        /// exist, it is first created with a value of 0 (thus, the new value is 1).
        /// </summary>
        /// <param name="counter">the name of the counter</param>
        void IncrementCounter(string counter);

        /// <summary>
        ///  Decrements the value of a counter by one. If this counter does not yet
        /// exist, it is first created with a value of 0 (thus, the new value is -1).
        /// </summary>
        /// <param name="counter">the name of the counter</param>
        void DecrementCounter(string counter);
    }
}