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
using System.ComponentModel;

namespace Summer.Batch.Common.Property
{
    /// <summary>
    /// Utility class to access properties on an object.
    /// </summary>
    public class PropertyAccessor
    {
        private readonly object _wrappedInstance;
        private readonly PropertyDescriptorCollection _propertyDescriptorCollection;

        /// <summary>
        /// Creates a property accessor with the specified wrapped instance.
        /// </summary>
        /// <param name="wrappedInstance">the object to wrap</param>
        public PropertyAccessor(object wrappedInstance)
        {
            _wrappedInstance = wrappedInstance;
            _propertyDescriptorCollection = TypeDescriptor.GetProperties(wrappedInstance);
        }

        /// <summary>
        /// Retrieves the value of a property on the wrapped instance. Allows path to reference nested properties.
        /// </summary>
        /// <param name="propertyPath">the path to the property to retrieve</param>
        /// <returns>the value of the property</returns>
        /// <exception cref="InvalidPropertyException">if the property does not exist</exception>
        public object GetProperty(string propertyPath)
        {
            var properties = propertyPath.Split('.');
            PropertyDescriptor property = null;
            var value = _wrappedInstance;
            var collection = _propertyDescriptorCollection;
            for (var i = 0; i < properties.Length && value != null; i++)
            {
                if (property != null)
                {
                    collection = property.GetChildProperties();
                }
                property = GetPropertyDescriptor(properties[i], collection);
                value = property.GetValue(value);
            }
            return value;
        }

        /// <summary>
        /// Retrieves a property descriptor from its name.
        /// </summary>
        /// <param name="propertyName">the name of a property</param>
        /// <param name="collection">the property collection to search</param>
        /// <returns>the corresponding property descriptor</returns>
        /// <exception cref="InvalidPropertyException">if the property does not exist</exception>
        private PropertyDescriptor GetPropertyDescriptor(string propertyName, PropertyDescriptorCollection collection)
        {
            var property = collection.Find(propertyName, true);
            if (property == null)
            {
                throw new InvalidPropertyException(string.Format("Invalid property {0} on type {1}", propertyName, _wrappedInstance.GetType()),
                    _wrappedInstance.GetType(), propertyName);
            }
            return property;
        }
    }
}