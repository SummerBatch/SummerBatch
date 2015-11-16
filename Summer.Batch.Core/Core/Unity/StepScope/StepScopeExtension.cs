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
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Summer.Batch.Core.Scope;

namespace Summer.Batch.Core.Unity.StepScope
{
    /// <summary>
    /// Unity extension to handle the step scope.
    /// It adds the <see cref="StepScopeStrategy"/> at the <see cref="UnityBuildStage.PreCreation"/> stage
    /// and registers to the <see cref="ExtensionContext.Registering"/> event to synchronize registration
    /// of dependencies in the step scope with <see cref="StepScopeSynchronization"/>.
    /// </summary>
    public class StepScopeExtension : UnityContainerExtension
    {
        /// <summary>
        /// Adds the <see cref="StepScopeStrategy"/> to the container.
        /// </summary>
        protected override void Initialize()
        {
            Context.Strategies.AddNew<StepScopeStrategy>(UnityBuildStage.PreCreation);
            Context.Registering += CheckStepScope;
        }

        private void CheckStepScope(object sender, RegisterEventArgs args)
        {
            var isStepScope = args.LifetimeManager is StepScopeLifetimeManager;
            // First we need to check any registered dependency with the same registered type and name
            Container.Registrations.Where(r => r.RegisteredType != (args.TypeFrom ?? args.TypeTo) &&
                r.MappedToType == args.TypeTo && r.Name == args.Name).ForEach(r =>
            {
                if (StepScopeSynchronization.IsStepScope(r.MappedToType, r.Name))
                {
                    // if the previously registered dependency is in step scope, a newly defined
                    // liftetime manager takes precedence. If no lifetime manager is defined for
                    // the new dependency, it is in step scope.
                    if (args.LifetimeManager == null)
                    {
                        isStepScope = true;
                    }
                    if (!isStepScope)
                    {
                        StepScopeSynchronization.RemoveScopeDependency(r.RegisteredType, r.Name);
                        StepScopeSynchronization.RemoveScopeDependency(r.MappedToType, r.Name);
                    }
                }
                else if (isStepScope)
                {
                    // if the previously registered dependency is not in step scope but the new dependency
                    // is, the previously registered dependency is put in step scope.
                    if (r.RegisteredType != r.MappedToType)
                    {
                        StepScopeSynchronization.AddStepScopeDependency(r.RegisteredType, r.Name, Container, r.MappedToType);
                    }
                    StepScopeSynchronization.AddStepScopeDependency(r.MappedToType, r.Name, Container, r.MappedToType);
                }
            });
            // Finally, we register the new dependency in the step scope if required
            if (isStepScope)
            {
                if (args.TypeFrom != null)
                {
                    StepScopeSynchronization.AddStepScopeDependency(args.TypeFrom, args.Name, Container, args.TypeTo);
                }
                StepScopeSynchronization.AddStepScopeDependency(args.TypeTo, args.Name, Container, args.TypeTo);
            }
        }
    }
}