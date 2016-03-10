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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Template
{
    /// <summary>
    /// Abstract base class for implementations of <see cref="ITemplateLineAggregator{T}"/>.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public abstract class AbstractTemplateLineAggregator<T> : ITemplateLineAggregator<T>, IInitializationPostOperations, IHeaderWriter, IFooterWriter
    {
        private readonly IDictionary<string, string> _templateLines = new Dictionary<string, string>();

        /// <summary>
        /// A resource to the template file.
        /// </summary>
        public IResource Template { get; set; }

        /// <summary>
        /// The encoding of the template file. Default is <see cref="Encoding.Default"/>.
        /// </summary>
        public Encoding InputEncoding { get; set; }

        /// <summary>
        /// The culture to use when formatting lines. Default is <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// The line separator to use when aggregating several lines. Default is <see cref="Environment.NewLine"/>.
        /// </summary>
        public string LineSeparator { get; set; }

        /// <summary>
        /// The id of the template line to use.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Default constructor that sets the default values.
        /// </summary>
        protected AbstractTemplateLineAggregator()
        {
            InputEncoding = Encoding.Default;
            Culture = CultureInfo.CurrentCulture;
            LineSeparator = Environment.NewLine;
        }

        /// <summary>
        /// Post Initialization operation.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Template, "Template must not be null.");
            ParseTemplate();
        }

        /// <summary>
        /// Transforms an item into a line using a template read from a file.
        /// </summary>
        /// <param name="item">the item to transform</param>
        /// <returns>the line corresponding to the given item</returns>
        public string Aggregate(T item)
        {
            return GetFormattedLine(TemplateId, GetParameters(item));
        }

        /// <summary>
        /// Formats and return the specified line.
        /// </summary>
        /// <param name="lineId">the id of the line to get</param>
        /// <param name="parameters">the parameters for formatting the line</param>
        /// <returns>the formatted line</returns>
        protected string GetFormattedLine(string lineId, IEnumerable<object> parameters)
        {
            string line;
            if (!_templateLines.TryGetValue(lineId, out line))
            {
                throw new InvalidOperationException("Invalid template line id: " + lineId);
            }
            return string.Format(Culture, line, parameters.ToArray());
        }

        /// <summary>
        /// Returns the parameters to use when formatting the template line.
        /// </summary>
        /// <param name="item">the item to display</param>
        /// <returns>the parameters of the current line corresponding to <paramref name="item"/></returns>
        protected abstract IEnumerable<object> GetParameters(T item);

        /// <summary>
        /// Parses the template to get the lines.
        /// </summary>
        private void ParseTemplate()
        {
            using (var reader = new StreamReader(Template.GetInputStream(), InputEncoding))
            {
                string line;
                string currentLineName = null;
                StringBuilder builder = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { ':' }, 2);
                    var lineName = split[0].Trim();
                    if (lineName == string.Empty)
                    {
                        if (currentLineName == null)
                        {
                            throw new InvalidOperationException("The first template line must be named.");
                        }
                        builder.Append(LineSeparator);
                    }
                    else
                    {
                        if (currentLineName != null)
                        {
                            _templateLines[currentLineName] = builder.ToString();
                        }
                        builder = new StringBuilder();
                        currentLineName = lineName;
                    }
                    builder.Append(split[1]);
                }
                if (builder != null)
                {
                    _templateLines[currentLineName] = builder.ToString();
                }
            }
        }

        /// <summary>
        /// Subclasses may override this 
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteHeader(TextWriter writer)
        {
            //left empty on purpose
        }

        /// <summary>
        /// Subclasses may override this
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteFooter(TextWriter writer)
        {
            //left empty on purpose
        }
    }
}