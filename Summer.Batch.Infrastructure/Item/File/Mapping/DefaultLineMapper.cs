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
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.Infrastructure.Item.File.Mapping
{
    /// <summary>
    /// Implementation of <see cref="T:ILineMapper"/> that maps the line in two phases:
    /// first the line is separated in fields using a <see cref="ILineTokenizer"/>, then
    /// the resulting <see cref="IFieldSet"/> is mapped to an entity using a <see cref="T:IFieldSetMapper"/>.
    /// </summary>
    /// <typeparam name="T">the type of the mapped entities</typeparam>
    public class DefaultLineMapper<T> : ILineMapper<T>
    {
        /// <summary>
        /// The tokenizer that creates a <see cref="IFieldSet"/> from the line.
        /// </summary>
        public ILineTokenizer Tokenizer { get; set; }

        /// <summary>
        /// The mapper that creates the entity from an <see cref="IFieldSet"/>.
        /// </summary>
        public IFieldSetMapper<T> FieldSetMapper { get; set; }

        /// <summary>
        /// Takes a line and returns the corresponding item.
        /// </summary>
        /// <param name="line">the line to map</param>
        /// <param name="lineNumber">the number of the line</param>
        /// <returns>an item corresponding to the given line</returns>
        public T MapLine(string line, int lineNumber)
        {
            return FieldSetMapper.MapFieldSet(Tokenizer.Tokenize(line));
        }
    }
}