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
using Microsoft.Practices.Unity;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Common.Proxy;

namespace Summer.Batch.Core.Unity.StepScope
{
    /// <summary>
    /// Implementation of <see cref="LifetimeManager"/> for step scope. It relies on
    /// <see cref="StepSynchronizationManager"/> to store and retrieve the instance.
    /// </summary>
    public class StepScopeLifetimeManager : LifetimeManager
    {
        /// <summary>
        /// The unique name of the instance.
        /// </summary>
        private readonly string _name = Guid.NewGuid().ToString("D");

        /// <summary>
        /// Retrieves the stored object if it exists.
        /// </summary>
        /// <returns>the required object or null if it not present for the current step</returns>
        public override object GetValue()
        {
            return Context.GetAttribute(_name);
        }

        /// <summary>
        /// Stores an object for the current step.
        /// </summary>
        /// <param name="newValue">the object to store</param>
        public override void SetValue(object newValue)
        {
            if (!(newValue is IProxyObject))
            {
                Context.SetAttribute(_name, newValue);
            }
        }

        /// <summary>
        /// Removes the instance from the context.
        /// </summary>
        public override void RemoveValue()
        {
            Context.RemoveAttribute(_name);
        }

        /// <summary>
        /// Convenience property to get the context from <see cref="StepSynchronizationManager"/>.
        /// </summary>
        private static StepContext Context
        {
            get
            {
                var context = StepSynchronizationManager.GetContext();
                if (context == null)
                {
                    throw new InvalidOperationException("No context available for step scope.");
                }
                return context;
            }
        }
    }
}