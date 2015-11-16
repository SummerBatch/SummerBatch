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

using System;

namespace Summer.Batch.Core.Listener
{
    /// <summary>
    /// A common interface for listener meta data enumerations
    /// No support for attributes.
    /// </summary>
    public interface IListenerMetaData
    {
        /// <summary>
        /// Returns the method name.
        /// </summary>
        /// <returns></returns>
        string GetMethodName();

        /// <summary>
        /// Returns the listener interface.
        /// </summary>
        /// <returns></returns>
        Type GetListenerInterface();

        /// <summary>
        /// Returns the property name.
        /// </summary>
        /// <returns></returns>
        string GetPropertyName();

        /// <summary>
        /// Return the param types.
        /// </summary>
        /// <returns></returns>
        Type[] GetParamTypes();

    }
}