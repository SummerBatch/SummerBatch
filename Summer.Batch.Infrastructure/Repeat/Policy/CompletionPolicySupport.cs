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
using Summer.Batch.Infrastructure.Repeat.Context;

namespace Summer.Batch.Infrastructure.Repeat.Policy
{
    /// <summary>
    ///  Very simple base class for ICompletionPolicy implementations.
    /// </summary>
    public class CompletionPolicySupport : ICompletionPolicy
    {
        /// <summary>
        /// If exit status is not continuable returns <code>true</code>, otherwise
	    /// delegates to  #IsComplete(IRepeatContext).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool IsComplete(IRepeatContext context, RepeatStatus result)
        {
            if (result != null && !result.IsContinuable())
            {
                return true;
            }
            return IsComplete(context);
            
        }

        /// <summary>
        /// Always true.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool IsComplete(IRepeatContext context)
        {
            return true;
        }

        /// <summary>
        /// Builds a new IRepeatContext and returns it.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual IRepeatContext Start(IRepeatContext parent)
        {
            return new RepeatContextSupport(parent);
        }

        /// <summary>
        ///  Increments the context so the counter is up to date. Do nothing else.
        /// </summary>
        /// <param name="context"></param>
        public void Update(IRepeatContext context)
        {
            var support = context as RepeatContextSupport;
            if (support != null) {
			    support.Increment();
		    }
        }
    }
}