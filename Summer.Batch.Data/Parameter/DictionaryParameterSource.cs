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

namespace Summer.Batch.Data.Parameter
{
    /// <summary>
    /// A <see cref="IQueryParameterSource"/> based on a dictionary.
    /// </summary>
    public class DictionaryParameterSource : IQueryParameterSource
    {
        /// <summary>
        /// The dictionary containing the parameter values.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Returns the value of a parameter.
        /// </summary>
        /// <param name="name">the name of a parameter</param>
        /// <returns>the value of the given parameter</returns>
        public object this[string name]
        {
            get
            {
                object result;
                Parameters.TryGetValue(name, out result);
                return result;
            }
        }
    }
}