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
using System;

namespace Summer.Batch.Infrastructure.Repeat
{
    /// <summary>
    /// Enumeration of possible Repeat statuses (either "Continuable" or "Finished")
    /// </summary>
    public class RepeatStatus : IComparable<RepeatStatus>
    {
        /// <summary>
        /// Indicates that processing can continue.
        /// </summary>
        public readonly static RepeatStatus Continuable = new RepeatStatus(true);

        /// <summary>
        /// Indicates that processing is finished (either successful or unsuccessful)
        /// </summary>
        public readonly static RepeatStatus Finished = new RepeatStatus(false);

        private readonly bool _continuable;

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="continuable"></param>
        private RepeatStatus(bool continuable)
        {
            _continuable = continuable;
        }

        /// <summary>
        /// Should we continue ?
        /// </summary>
        /// <param name="continuable"></param>
        /// <returns></returns>
        public static RepeatStatus ContinueIf(bool continuable)
        {
            return continuable ? Continuable : Finished;
        }

        /// <summary>
        /// Test for equality with Continuable.
        /// </summary>
        /// <returns></returns>
        public bool IsContinuable()
        {
            return this == Continuable;
        }

        /// <summary>
        /// Logical and.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public RepeatStatus And(bool value)
        {
            return value && _continuable ? Continuable : Finished;
        }

        /// <summary>
        ///  Compares RepeatStatus so that one that is continuable ranks lowest.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(RepeatStatus other)
        {
            if (other == null)
            {
                return 1;
            }
            if ((IsContinuable() && other.IsContinuable())
                    || (!IsContinuable() && !other.IsContinuable()))
            {
                return 0;
            }
            if (IsContinuable())
            {
                return -1;
            }
            return 1;   
        }
    }
}
