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
using Summer.Batch.Common.Property;
using System;

namespace Summer.Batch.Data.Parameter
{
    /// <summary>
    /// Implementation of <see cref="IQueryParameterSource"/> that retrieves parameter values by reading
    /// the properties of an object. The property lookup is case insensitive.
    /// </summary>
    public class PropertyParameterSource : IQueryParameterSource
    {
        private readonly PropertyAccessor _propertyAccessor;

        /// <summary>
        /// Returns the value of a parameter by reading the property of the wrapped object.
        /// </summary>
        /// <param name="name">the name of a parameter</param>
        /// <returns>the value of the given parameter</returns>
        public object this[string name]
        {
            get {
                var result = _propertyAccessor.GetProperty(name);
                if (result == null)
                {
                    result = System.DBNull.Value;
                }
                return result;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="item">the wrapped instance that will be used to retrieve parameter values</param>
        public PropertyParameterSource(object item)
        {
            _propertyAccessor = new PropertyAccessor(item);
        }
    }
}