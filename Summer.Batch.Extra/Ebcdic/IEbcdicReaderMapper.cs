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
using System.Collections.Generic;
using Summer.Batch.Extra.Copybook;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// An EbcdicReaderMapper maps a list of fields, corresponding to an EBCDIC
    /// record, to a business object.
    /// </summary>
    /// <typeparam name="TT"></typeparam>
    public interface IEbcdicReaderMapper<out TT>
    {
        /// <summary>
        /// Converts the content of a list of values into a business object.
        /// </summary>
        /// <param name="values">the list of values to map</param>
        /// <param name="itemCount">the record line number, starting at 0.</param>
        /// <returns>The mapped object</returns>
        TT Map(IList<object> values, int itemCount);

        /// <summary>
        ///  Sets the record format map to use for mapping
        /// </summary>
        RecordFormatMap RecordFormatMap { set; }

        /// <summary>
        /// Sets the date parser
        /// </summary>
        IDateParser DateParser { set; }

        /// <summary>
        ///  The getter for the distinguished pattern.
        /// </summary>
        string DistinguishedPattern { get; }

    }
}