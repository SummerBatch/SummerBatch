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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Delegating
{
    /// <summary>
    /// This class delegates reading to the inner reader and 
    /// accumulates the records while no rupture is found, using
    /// the supplied rupture definition (list of fields separated
    /// by commas).
    /// </summary>
    /// <typeparam name="T">The type of objects read by the reader</typeparam>
    public class DelegatingGroupReader<T> : IItemStreamReader<List<T>> where T : class
    {

        #region Attributes
        /// <summary>
        /// The delegate, i.e. the real reader that is grouped.
        /// </summary>
        public IItemReader<T> Delegate { private get; set; }

        /// <summary>
        /// The different fields that can be a group rupture. 
        /// </summary>
        private string[][] _ruptureFields;

        /// <summary>
        ///The property descriptors for rupture fields.
        /// </summary>
        private PropertyDescriptor[][] _ruptureProperties;

        /// <summary>
        /// The buffer, containing the record to be processed next time. 
        /// </summary>
        private T _buffer;

        /// <summary>
        ///The buffer values, for comparison sake.  
        /// </summary>
        private List<object> _bufferRuptureValues = new List<object>();

        /// <summary>
        /// Boolean to know if this is the first read.
        /// </summary>
        private bool _isFirst = true;
        #endregion

        /// <summary>
        /// Registers the names of the relevant fields to check a rupture.
        /// </summary>
        public string RuptureFields
        {
            set
            {
                var split = Regex.Split(value, ",");
                _ruptureFields = new string[split.Length][];
                _ruptureProperties = new PropertyDescriptor[split.Length][];
                for (var i = 0; i < split.Length; i++)
                {
                    _ruptureFields[i] = Regex.Split(split[i], @"\.");
                    _ruptureProperties[i] = new PropertyDescriptor[_ruptureFields[i].Length];
                }
            }
        }

        /// <summary>
        /// Simply delegating to the inner grouped reader
        /// </summary>
        /// <param name="executionContext"></param>
        public void Open(ExecutionContext executionContext)
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Open(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner grouped reader
        /// </summary>
        /// <param name="executionContext"></param>
        public void Update(ExecutionContext executionContext)
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Update(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner grouped reader
        /// </summary>
        public void Close()
        {
            var stream = Delegate as IDisposable;
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Flushes the delegate writer if it is an instance of <see cref="IItemStream"/>.
        /// </summary>
        public void Flush()
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Flush();
            }
        }

        /// <summary>
        /// Read through the delegate, grouping records in a list.
        /// </summary>
        /// <returns> the list of read records</returns>
        /// <exception cref="Exception"></exception>
        public List<T> Read()
        {
            List<T> toReturn = null;

            if (_isFirst)
            {
                // Read first record through delegate. 
                _buffer = Delegate.Read();
                _isFirst = false;
                if (_buffer != null)
                {
                    // Compute the values for the rupture fields of the buffer
                    _bufferRuptureValues = GetRuptureValues(_buffer);
                }
            }

            // If current record is null, we have reached the end of the step
            // We must not read next, which would overflow
            if (_buffer != null)
            {
                toReturn = new List<T> { _buffer };
                var rupture = false;
                T newRecord;
                List<object> values = null;
                // Loop until a rupture is found or last record is reached
                do
                {
                    // Read next record through delegate
                    newRecord = Delegate.Read();
                    if (newRecord != null)
                    {
                        // Compute the values for the rupture fields of the record
                        values = GetRuptureValues(newRecord);
                        // Decide if rupture. If not rupture, record is in the same group
                        // Add it to the return list and continue looping
                        rupture = IsRupture(values);
                    }
                    if (!rupture && newRecord != null)
                    {
                        toReturn.Add(newRecord);
                    }
                }
                while (newRecord != null && !rupture);
                // When a rupture is found or end is reached, store the last
                // record, i.e. the one that did not belong to the group, as 
                // the new buffer for next group
                _buffer = newRecord;
                _bufferRuptureValues = values;
            }

            return toReturn;
        }

        /// <summary>
        /// Find the values of the rupture fields in the parameter object.
        /// </summary>
        /// <param name="obj">the object to analyze</param>
        /// <returns>the list of rupture values</returns>
        private List<object> GetRuptureValues(T obj)
        {
            var values = new List<object>();
            for (var i = 0; i < _ruptureFields.Length; i++)
            {
                object currentObject = obj;
                for (var j = 0; j < _ruptureFields[i].Length && currentObject != null; j++)
                {
                    var propertyDescriptor = _ruptureProperties[i][j];
                    if (propertyDescriptor == null)
                    {
                        var propertyCollection = TypeDescriptor.GetProperties(currentObject.GetType());
                        propertyDescriptor = propertyCollection.Find(_ruptureFields[i][j], true);
                        if (propertyDescriptor == null)
                        {
                            throw new ArgumentException(
                                string.Format("Cannot find field {0}", ToString(_ruptureFields[i])));
                        }
                        _ruptureProperties[i][j] = propertyDescriptor;
                    }
                    currentObject = propertyDescriptor.GetValue(currentObject);
                }
                values.Add(currentObject);
            }
            return values;
        }

        /// <summary>
        /// Check if there is a rupture comparing the values in parameter to
        /// the corresponding values in the buffer.
        /// </summary>
        /// <param name="values">the values to compare</param>
        /// <returns>whether there is a rupture</returns>
        private bool IsRupture(List<object> values)
        {
            return !values.SequenceEqual(_bufferRuptureValues);
        }

        /// <summary>
        /// Dump field to string
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string ToString(string[] field)
        {
            var sb = new StringBuilder(field[0]);
            for (var i = 1; i < field.Length; i++)
            {
                sb.Append('.').Append(field[i]);
            }
            return sb.ToString();
        }

        #region Disposable pattern

        /// <summary>
        /// Releases the resources used by the reader.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Actually disposes the current object.
        /// </summary>
        /// <param name="disposing">
        /// Indicates whether the method was invoked from the <see cref="IDisposable.Dispose"/>
        /// implementation or from the finalizer
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion
    }
}