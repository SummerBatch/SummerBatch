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

using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;

namespace Summer.Batch.Core.Job.Flow.Support
{
    /// <summary>
    /// Sorts by decreasing specificity of pattern, based on just counting
    /// wildcards (with * taking precedence over ?). If wildcard counts are equal
    /// then falls back to alphabetic comparison. Hence * &gt; foo* &gt; ??? &gt;
    /// fo? &gt; foo.
    /// </summary>
    public class DefaultStateTransitionComparer : IComparer<StateTransition>
    {
        /// <summary>
        /// State Transition Comparer constant.
        /// </summary>
        public const string StateTransitionComparer = "batch_state_transition_comparer";
        
        /// <summary>
        /// @see IComparer#Compare .
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public int Compare(StateTransition arg0, StateTransition arg1)
        {
            string value = arg1.Pattern;
            if (arg0.Pattern.Equals(value))
            {
                return 0;
            }
            int patternCount = StringUtils.CountOccurrencesOf(arg0.Pattern, "*");
            int valueCount = StringUtils.CountOccurrencesOf(value, "*");
            if (patternCount > valueCount)
            {
                return 1;
            }
            if (patternCount < valueCount)
            {
                return -1;
            }
            patternCount = StringUtils.CountOccurrencesOf(arg0.Pattern, "?");
            valueCount = StringUtils.CountOccurrencesOf(value, "?");
            if (patternCount > valueCount)
            {
                return 1;
            }
            if (patternCount < valueCount)
            {
                return -1;
            }
            return string.Compare(arg0.Pattern, value, StringComparison.Ordinal);
        }
    }
}