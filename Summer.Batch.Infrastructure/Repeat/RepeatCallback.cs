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

namespace Summer.Batch.Infrastructure.Repeat
{
    /// <summary>
    /// NB : Refers to the RepeatCallback java interface, with single method (doInIteration)
    /// 
    /// Implementations return true if they can continue processing - e.g. there
    /// is a data source that is not yet exhausted. Exceptions are not necessarily
    /// fatal - processing might continue depending on the Exception type and the
    /// implementation of the caller.        
    /// </summary>
    /// <param name="context">the current context passed in by the caller.</param>
    /// <returns>an RepeatStatus which is continuable if there is (or may be) more data to process.</returns>
    /// <exception cref="Exception">&nbsp;</exception>
    public delegate RepeatStatus RepeatCallback(IRepeatContext context);
}
