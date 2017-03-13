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
using System.Collections.Generic;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Implementation of <see cref="ILineTokenizer"/> for lines with fixed-length format.
    /// Columns are specified using ranges.
    /// </summary>
    public class FixedLengthTokenizer : AbstractLineTokenizer
    {
        private Range[] _ranges;
        private int _maxRange;
        private bool _open;

        /// <summary>
        /// The ranges of the columns to extract
        /// </summary>
        public Range[] Columns
        {
            get { return _ranges; }
            set
            {
                _ranges = value;
                CalculateMaxRange();
            }
        }

        private string _columnDefs;

        /// <summary>
        /// Define ranges of columns to extract using a dash and comma delimited string
        /// </summary>
        public string ColumnDefs
        {
            get { return _columnDefs; }
            set
            {
                _columnDefs = value;

                if (_columnDefs.Trim().Length == 0)
                {
                    throw new InvalidOperationException("ColumnDefs is empty");
                }

                List<Range> ranges = new List<Range>();

                foreach (var range in _columnDefs.Split(','))
                {
                    var rangeDef = range.Split('-');

                    if (rangeDef.Length > 2)
                    {
                        throw new InvalidOperationException("ColumnDefs definition is invalid");
                    }

                    Range rangeItem = null;

                    if (rangeDef.Length > 1)
                    {
                        rangeItem = new Range(Convert.ToInt32(rangeDef[0]), Convert.ToInt32(rangeDef[1]));
                    }
                    else
                    {
                        rangeItem = new Range(Convert.ToInt32(rangeDef[0]));
                    }

                    ranges.Add(rangeItem);
                }

                if (ranges.Count == 0)
                {
                    throw new InvalidOperationException("ColumnDefs definition is invalid");
                }

                _ranges = ranges.ToArray();
                CalculateMaxRange();
            }
        }


        /// <summary>
        /// Method that does the actual tokenizing.
        /// </summary>
        /// <param name="line">the line to tokenize</param>
        /// <returns>a list of field values</returns>
        protected override IList<string> DoTokenize(string line)
        {
            var tokens = new List<string>();

            if (line.Length < _maxRange && Strict)
            {
                throw new InvalidOperationException(string.Format("Line is shorter than max range {0}", _maxRange));
            }

            if (!_open && line.Length > _maxRange && Strict)
            {
                throw new InvalidOperationException(string.Format("Line is longer than max range {0}", _maxRange));
            }

            foreach (var range in _ranges)
            {
                var startPos = range.Min - 1;
                var endPos = range.Max;

                if (line.Length >= endPos)
                {
                    tokens.Add(line.Substring(startPos, endPos - startPos));
                }
                else if (line.Length >= startPos)
                {
                    tokens.Add(line.Substring(startPos));
                }
                else
                {
                    tokens.Add(string.Empty);
                }
            }

            return tokens;
        }

        /// <summary>
        /// Calculates the max range.
        /// </summary>
        private void CalculateMaxRange()
        {
            if (_ranges == null || _ranges.Length == 0)
            {
                _maxRange = 0;
                return;
            }

            var maxRange = 0;
            var maxRangeIndex = 0;

            for (var i = 0; i < _ranges.Length; i++)
            {
                var upperBound = _ranges[i].HasMaxValue ? _ranges[i].Max : _ranges[i].Min;
                if (upperBound > maxRange)
                {
                    maxRange = upperBound;
                    maxRangeIndex = i;
                }
            }

            _maxRange = maxRange;
            _open = !_ranges[maxRangeIndex].HasMaxValue;
        }
    }
}