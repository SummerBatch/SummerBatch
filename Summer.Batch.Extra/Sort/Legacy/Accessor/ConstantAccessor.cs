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

namespace Summer.Batch.Extra.Sort.Legacy.Accessor
{
    /// <summary>
    /// Implementation of <see cref="IAccessor{T}"/> that returns a constant.
    /// <see cref="Set"/> is not supported.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class ConstantAccessor<T> : IAccessor<T>
    {
        /// <summary>
        /// The constant to return.
        /// </summary>
        public T Constant { get; set; }

        /// <summary>
        /// Returns the constant.
        /// </summary>
        /// <param name="record">ignored</param>
        /// <returns>a constant</returns>
        public T Get(byte[] record)
        {
            return Constant;
        }

        /// <exception cref="InvalidOperationException">&nbsp;this operation is not supported and will always throw this exception</exception>
        public void Set(byte[] record, T value)
        {
            throw new InvalidOperationException("Cannot set value on a constant accessor");
        }
    }
}