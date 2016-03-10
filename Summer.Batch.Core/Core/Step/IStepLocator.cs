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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

//   This file has been modified.
//   Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;

namespace Summer.Batch.Core.Step
{
    /// <summary>
    ///  Interface for locating a Step instance by name.
    /// </summary>
    public interface IStepLocator
    {
        /// <summary>
        /// Returns the step names collection.
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetStepNames();
        
        /// <summary>
        /// Returns a step given its name.
        /// Thorws a NoSuchStepException if step could not be found.
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchStepException">&nbsp;</exception>
        IStep GetStep(string stepName);
    }
}
