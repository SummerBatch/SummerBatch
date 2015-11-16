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
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// A class to represent ranges. The range minimum and maximum can have values from 1 to <see cref="int.MaxValue"/>.
    /// If the maximum is <see cref="UnboundMaximum"/>, it is considered to be unbound on the right side.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// The value that specifies there is no maximum.
        /// </summary>
        public const int UnboundMaximum = int.MaxValue;

        private readonly int _min;
        private readonly int _max;

        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        public int Min
        {
            get { return _min; }
        }

        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        public int Max
        {
            get { return _max; }
        }

        /// <summary>
        /// Whether there is a maximum value or the range is unbound on the left side.
        /// </summary>
        public bool HasMaxValue
        {
            get { return _max != UnboundMaximum; }
        }

        /// <summary>
        /// Creates a new range.
        /// </summary>
        /// <param name="min">the minimum value for the range</param>
        /// <param name="max">the maximum value for the range, <see cref="UnboundMaximum"/> if unspecified</param>
        public Range(int min, int max = UnboundMaximum)
        {
            Assert.IsTrue(min > 0, "Min value must be strictly positive.");
            Assert.IsTrue(min <= max, "Min value must be lower or equal to max value.");
            _min = min;
            _max = max;
        }

        /// <returns>
        /// A string that represents the current range.
        /// </returns>
        public override string ToString()
        {
            return HasMaxValue ? _min + "-" + _max : _min.ToString();
        }
    }
}