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
    ///  Used to manage objects cache in Jobs/Steps. The execution context contains
    /// information on the current Job/Step being executed.
    /// </summary>
    public class ContextManager : IContextManager
    {
        /// <summary>
        /// cache
        /// </summary>
        public ExecutionContext Context { get; set; }

        /// <summary>
        /// Store an object inside the cache
        /// </summary>
        /// <param name="key">object key to store (should normally be a string)</param>
        /// <param name="record">object to store</param>
        public void PutInContext(object key, object record)
        {
            string strKey = key.ToString();
            if (record == null)
            {
                Context.Remove(strKey);
            }
            else
            {
                Context.Put(strKey, record);
            }
        }

        /// <summary>
        /// Check if the given key is used in the cache
        /// </summary>
        /// <param name="key">key of the object to retrieve</param>
        /// <returns>whether the key is used in the cache or not</returns>
        public bool ContainsKey(object key)
        {
            return Context.ContainsKey(key.ToString());
        }

        /// <summary>
        /// Retrieves an object from the cache
        /// </summary>
        /// <param name="key">key of the object to retrieve</param>
        /// <returns>the object</returns>
        public object GetFromContext(object key)
        {
            return Context.Get(key.ToString());
        }

        /// <summary>
        /// CLear the cache.
        /// </summary>
        public void Empty()
        {
            Context.Empty();
        }

        /// <summary>
        /// Duimps the cache content
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            return Context.ToString();
        }

        /// <summary>
        /// Sets the value of the given counter
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="value"></param>
        public void SetCounter(string counter, long value)
        {
            Context.PutLong(counter, value);
        }

        /// <summary>
        /// Returns the value of the given counter (0 if not found)
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        public long GetCounter(string counter)
        {
            if (Context.ContainsKey(counter))
            {
                return Context.GetLong(counter);
            }
            else
            {
                return 0L;
            }
        }

        /// <summary>
        /// Increment the value of the given counter
        /// </summary>
        /// <param name="counter"></param>
        public void IncrementCounter(string counter)
        {
            Context.PutLong(counter, GetCounter(counter) + 1);
        }

        /// <summary>
        /// Decrement the valeu of the given counter
        /// </summary>
        /// <param name="counter"></param>
        public void DecrementCounter(string counter)
        {
            Context.PutLong(counter, GetCounter(counter) - 1);
        }
    }
}