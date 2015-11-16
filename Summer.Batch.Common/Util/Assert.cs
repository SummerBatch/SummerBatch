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
using System.Linq;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Static Assert Helper.
    /// </summary>
    public static class Assert
    {

        /// <summary>
        /// Asserts a boolean expression.
        /// </summary>
        /// <param name="expression">a boolean expression</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the expression is evaluated to <code>false</code></exception>
        public static void IsTrue(bool expression, string message = null)
        {
            if (!expression)
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this expression must be true");
            }
        }

        /// <summary>
        /// Asserts that an object is null.
        /// </summary>
        /// <param name="obj">the object to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the object is not null</exception>
        public static void IsNull(object obj, string message = null)
        {
            if (obj != null)
            {
                throw new ArgumentException(message ?? "[Assertion failed] - the object argument must be null");
            }
        }

        /// <summary>
        /// Asserts that an object is not null.
        /// </summary>
        /// <param name="obj">the object to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentNullException">if the object is null</exception>
        public static void NotNull(object obj, string message = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message ?? "[Assertion failed] - this argument is required; it must not be null");
            }
        }

        /// <summary>
        /// Asserts that a string is not null and is not empty.
        /// </summary>
        /// <param name="text">the string to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the string is null or empty</exception>
        public static void HasLength(string text, string message = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this String argument must have length; it must not be null or empty");
            }
        }

        /// <summary>
        /// Asserts that a string is not null and contains at least one non-whitespace character.
        /// </summary>
        /// <param name="text">the string to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the string is null or does not contain text</exception>
        public static void HasText(string text, string message = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this String argument must have text; it must not be null, empty, or blank");
            }
        }

        /// <summary>
        /// Asserts that a string does not contain a substring.
        /// </summary>
        /// <param name="textToSearch">the string to search</param>
        /// <param name="substring">the substring to find</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the substring is found in text</exception>
        public static void DoesNotContain(string textToSearch, string substring, string message = null)
        {
            if (!string.IsNullOrEmpty(textToSearch) && !string.IsNullOrEmpty(substring) &&
                    textToSearch.Contains(substring))
            {
                throw new ArgumentException(message ?? 
                    string.Format("[Assertion failed] - this String argument must not contain the substring [{0}]",substring));
            }
        }


        /// <summary>
        /// Asserts that an array is not null and that its length is greater than zero.
        /// </summary>
        /// <param name="array">the array to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the array is null or empty</exception>
        public static void NotEmpty(object[] array, string message = null)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this array must not be empty: it must contain at least 1 element");
            }
        }

        /// <summary>
        /// Asserts that an array has no null elements.
        /// </summary>
        /// <param name="array">the array to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the array is null or contains a null element</exception>
        public static void NoNullElements(object[] array, string message = null)
        {
            if (null != array && array.Any(element => element == null))
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this array must not contain any null elements");
            }
        }

        /// <summary>
        /// Asserts that a dictionary is not empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the dictionary</typeparam>
        /// <param name="collection">the dictionary to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the collection is null or empty</exception>
        public static void NotEmpty<T>(ICollection<T> collection, string message = null)
        {
            if (collection == null || collection.Count == 0)
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this dictionary must not be empty: it must contain at least 1 element");
            }
        }

        /// <summary>
        /// Asserts that a dictionary is not empty.
        /// </summary>
        /// <typeparam name="TKey">the type of the keys</typeparam>
        /// <typeparam name="TValue">the type of the values</typeparam>
        /// <param name="dictionary">the dictionary to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the dictionary is null or empty</exception>
        public static void NotEmpty<TKey, TValue>(IDictionary<TKey, TValue> dictionary, string message = null)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                throw new ArgumentException(message ?? "[Assertion failed] - this dictionary must not be empty; it must contain at least one entry");
            }
        }


        /// <summary>
        /// Asserts than an object is instance of a type.
        /// </summary>
        /// <param name="type">the type the object must be instance of</param>
        /// <param name="obj">the object to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="ArgumentException">if the object is not an instance of type</exception>
        public static void IsInstanceOf(Type type, object obj, string message = null)
        {
            NotNull(type, "Type to check against must not be null");
            if (!type.IsInstanceOfType(obj))
            {
                throw new ArgumentException(message ?? string.Format("Object of class [{0}] must be an instance of {1}", obj.GetType().Name, type.Name));
            }
        }

        /// <summary>
        /// Asserts that a supertype is assignable from a subtype.
        /// </summary>
        /// <param name="superType">the supertype to check</param>
        /// <param name="subType">the subtype to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// /// <exception cref="ArgumentException">if supertype is not assignable from subtype</exception>
        public static void IsAssignable(Type superType, Type subType, string message = null)
        {
            NotNull(superType, "Type to check against must not be null");
            if (subType == null || !superType.IsAssignableFrom(subType))
            {
                throw new ArgumentException(message ?? string.Format("{0} is not assignable to {1}", subType, superType));
            }
        }

        /// <summary>
        /// Asserts than an expression evaluates to <code>true</code>.
        /// </summary>
        /// <param name="expression">the expression to check</param>
        /// <param name="message">the message to use if assertion fails</param>
        /// <exception cref="InvalidOperationException">if expression evaluates to <code>false</code></exception>
        public static void State(bool expression, string message = null)
        {
            if (!expression)
            {
                throw new InvalidOperationException(message ?? "[Assertion failed] - this state invariant must be true");
            }
        }
    }
}
