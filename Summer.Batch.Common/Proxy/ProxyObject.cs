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
using System.Diagnostics.CodeAnalysis;

namespace Summer.Batch.Common.Proxy
{
    /// <summary>
    /// Implementation of <see cref="IProxyObject"/>.
    /// </summary>
    public class ProxyObject : IProxyObject
    {
        private object _instance;

        /// <summary>
        /// Retrieves the current underlying instance.
        /// </summary>
        /// <returns>the underlying instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "Method hidden from child types on purpose, to avoid name collisions.")]
        object IProxyObject.GetInstance()
        {
            if (_instance == null)
            {
                throw new ProxyException("Proxy error: no instance provided.");
            }
            return _instance;
        }

        /// <summary>
        /// Sets the underlying instance.
        /// Explicit implementation to avoid name collision with other interfaces.
        /// </summary>
        /// <param name="instance">the new instance</param>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "Method hidden from child types on purpose, to avoid name collisions.")]
        void IProxyObject.SetInstance(object instance)
        {
            _instance = instance;
        }
    }
}