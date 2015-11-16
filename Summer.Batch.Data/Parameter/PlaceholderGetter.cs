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
using System;

namespace Summer.Batch.Data.Parameter
{
    /// <summary>
    /// Default implementation of <see cref="IPlaceholderGetter"/>.
    /// </summary>
    public class PlaceholderGetter : IPlaceholderGetter
    {
        private readonly Func<string, string> _placeholderGetter;

        /// <summary>
        /// Whether the placeholders provided by this <see cref="IPlaceholderGetter"/>
        /// are named. If <code>false</code>, they are position-based.
        /// </summary>
        public bool Named { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="PlaceholderGetter"/>.
        /// </summary>
        /// <param name="placeholderGetter">a function that takes a parameter name and return its placeholder</param>
        /// <param name="named">whether the new <see cref="PlaceholderGetter"/> is named</param>
        public PlaceholderGetter(Func<string, string> placeholderGetter, bool named)
        {
            _placeholderGetter = placeholderGetter;
            Named = named;
        }

        /// <summary>
        /// Gets the correct placeholder for a query parameter.
        /// </summary>
        /// <param name="name">the name of the parameter</param>
        /// <returns>the corresponding parameter placeholder</returns>
        public string GetPlaceholder(string name)
        {
            return _placeholderGetter(name);
        }
    }
}
