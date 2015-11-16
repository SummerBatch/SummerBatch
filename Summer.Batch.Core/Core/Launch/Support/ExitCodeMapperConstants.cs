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

namespace Summer.Batch.Core.Launch.Support
{
    /// <summary>
    /// ExitCodeMapper Constants.
    /// </summary>
    public static  class ExitCodeMapperConstants
    {
        /// <summary>
        /// VmExitcodeCompleted constant.
        /// </summary>
        public const int VmExitcodeCompleted = 0;
        
        /// <summary>
        /// VmExitcodeGenericError constant.
        /// </summary>
        public const int VmExitcodeGenericError = 1;
        
        /// <summary>
        /// VmExitcodeJobError constant.
        /// </summary>
        public const int VmExitcodeJobError = 2;
        
        /// <summary>
        /// NoSuchJob constant.
        /// </summary>
        public const string NoSuchJob = "NO_SUCH_JOB";
        
        /// <summary>
        /// JobNotProvided constant.
        /// </summary>
        public const string JobNotProvided = "JOB_NOT_PROVIDED";
    }
}
