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

// This file has been modified.
// Original copyright notice :

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
namespace Summer.Batch.Infrastructure.Repeat.Exception
{
    /// <summary>
    /// Handler to allow strategies for re-throwing exceptions. Normally a
    /// CompletionPolicy will be used to decide whether to end a batch when
    /// there is no exception, and the ExceptionHandler is used to signal an
    /// abnormal ending - an abnormal ending would result in an
    /// ExceptionHandler throwing an exception. The caller will catch and
    /// re-throw it if necessary. 
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Deal with an exception during a batch - decide whether it should be re-thrown in the first place.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <exception cref="Exception">&nbsp;</exception>
        void HandleException(IRepeatContext context, System.Exception exception);
    }
}