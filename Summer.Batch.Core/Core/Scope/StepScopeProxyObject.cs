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
using System.Diagnostics.CodeAnalysis;
using Summer.Batch.Common.Proxy;

namespace Summer.Batch.Core.Scope
{
    /// <summary>
    /// Implementation of <see cref="IProxyObject"/> for step scope. The instance is retrieved
    /// using <see cref="StepScopeSynchronization.GetInstance"/>.
    /// </summary>
    public class StepScopeProxyObject : IProxyObject
    {
        /// <summary>
        /// Retrieves the current underlying instance.
        /// </summary>
        /// <returns>the underlying instance.</returns>
        /// <exception cref="InvalidOperationException">&nbsp;if no step is being executed in the current thread</exception>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "Method hidden from child types on purpose, to avoid name collisions.")]
        object IProxyObject.GetInstance()
        {
            return StepScopeSynchronization.GetInstance(this);
        }

        /// <summary>
        /// Not supported, the instance is managed by the step scope.
        /// </summary>
        /// <param name="instance">unused</param>
        /// <exception cref="NotSupportedException">&nbsp;alaways thrown</exception>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "Method hidden from child types on purpose, to avoid name collisions.")]
        void IProxyObject.SetInstance(object instance)
        {
            throw new NotSupportedException("Cannot set Instance on a StepScopeProxyObject");
        }
    }
}