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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Extension to unity container, to handle post-initializations operations.
    /// @see IInitializationPostOperations
    /// </summary>
    public class PostprocessingUnityExtension : UnityContainerExtension
    {
        /// <summary>
        /// Initialize
        /// </summary>
        protected override void Initialize()
        {
            Context.Strategies.AddNew<PostOperationsStrategy>(UnityBuildStage.PostInitialization);
        }
    }

    /// <summary>
    /// the <see cref="BuilderStrategy"/> used for this extension.
    /// </summary>
    class PostOperationsStrategy : BuilderStrategy
    {
        /// <summary>
        /// Used to invoke <see cref="IInitializationPostOperations#AfterPropertiesSet()"/> after
        /// initialization.
        /// </summary>
        /// <param name="context"></param>
        public override void PostBuildUp(IBuilderContext context)
        {
            IInitializationPostOperations toValidate = context.Existing as IInitializationPostOperations;
            if (toValidate != null)
            {
                toValidate.AfterPropertiesSet();
            }
        }
    }
}