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

namespace Summer.Batch.Common.Factory
{
    /// <summary>
    /// Interface to be implemented by beans that need to react once all 
    /// their properties have been set by a Factory: for example, to perform 
    /// custom initialization, or merely to check that all mandatory properties have been set.
    /// </summary>
    public interface IInitializationPostOperations
    {
        /// <summary>
        /// Invoked by a Factory after it has set all bean properties supplied.
        /// This method allows the object instance to perform initialization only possible when all 
        /// properties have been set and to throw an exception in the event of misconfiguration. 
        /// </summary>
        /// <exception cref="Exception">&nbsp;in the event of misconfiguration (such as failure to set an essential property) 
        /// or if initialization fails.</exception>
        void AfterPropertiesSet();
    }
}