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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.Collections;
using Summer.Batch.Common.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Common.Collections
{
    [TestClass]
    public class OrderedDictionaryTest
    {
        [TestMethod]
        public void TestAddContainsKeyGet()
        {
            var dictionary = new OrderedDictionary<string, string> {{"monkey", "banana"}, {"dog", "cat"}};
            Assert.IsTrue(dictionary.ContainsKey("monkey"));
            Assert.IsFalse(dictionary.ContainsKey("cat"));
            Assert.AreEqual(dictionary["monkey"], "banana");
        }

        [TestMethod]
        public void TestTryGetValueSet()
        {
            var dictionary = new OrderedDictionary<string, string> {{"monkey", "banana"}, {"dog", "cat"}};
            string value;
            Assert.IsTrue(dictionary.TryGetValue("dog", out value));
            Assert.AreEqual(value, "cat");
            dictionary["dog"] = "biscuit";
            Assert.AreEqual(value, "cat");
            Assert.AreEqual(dictionary["dog"], "biscuit");
        }

        [TestMethod]
        public void TestRemove()
        {
            var dictionary = new OrderedDictionary<string, string>
            {
                {"monkey", "banana"},
                {"dog", "cat"},
                {"cat", "mouse"},
                {"horse", "grass"},
                {"rabbit", "carrot"}
            };
            Assert.IsTrue(dictionary.ContainsKey("dog"));
            Assert.IsTrue(dictionary.ContainsKey("cat"));
            Assert.IsTrue(dictionary.Remove("dog"));
            Assert.IsFalse(dictionary.Remove("cow"));
            Assert.IsFalse(dictionary.ContainsKey("dog"));
            Assert.IsTrue(dictionary.ContainsKey("cat"));
        }

        [TestMethod]
        public void TestOrder()
        {
            var dictionary = new OrderedDictionary<string, string>
            {
                {"monkey", "banana"},
                {"dog", "cat"},
                {"cat", "mouse"},
                {"horse", "grass"},
                {"rabbit", "carrot"}
            };
            int i = 0;
            foreach (var pair in dictionary)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(pair.Key, "monkey");
                        Assert.AreEqual(pair.Value, "banana");
                        break;
                    case 1:
                        Assert.AreEqual(pair.Key, "dog");
                        Assert.AreEqual(pair.Value, "cat");
                        break;
                    case 2:
                        Assert.AreEqual(pair.Key, "cat");
                        Assert.AreEqual(pair.Value, "mouse");
                        break;
                    case 3:
                        Assert.AreEqual(pair.Key, "horse");
                        Assert.AreEqual(pair.Value, "grass");
                        break;
                    case 4:
                        Assert.AreEqual(pair.Key, "rabbit");
                        Assert.AreEqual(pair.Value, "carrot");
                        break;
                }
            }
            dictionary["dog"] = "biscuit";
            // Order should not change
            i = 0;
            foreach (var pair in dictionary)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(pair.Key, "monkey");
                        Assert.AreEqual(pair.Value, "banana");
                        break;
                    case 1:
                        Assert.AreEqual(pair.Key, "dog");
                        Assert.AreEqual(pair.Value, "biscuit");
                        break;
                    case 2:
                        Assert.AreEqual(pair.Key, "cat");
                        Assert.AreEqual(pair.Value, "mouse");
                        break;
                    case 3:
                        Assert.AreEqual(pair.Key, "horse");
                        Assert.AreEqual(pair.Value, "grass");
                        break;
                    case 4:
                        Assert.AreEqual(pair.Key, "rabbit");
                        Assert.AreEqual(pair.Value, "carrot");
                        break;
                }
            }
            dictionary.Remove("cat");
            // everything moves accordingly
            i = 0;
            foreach (var pair in dictionary)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(pair.Key, "monkey");
                        Assert.AreEqual(pair.Value, "banana");
                        break;
                    case 1:
                        Assert.AreEqual(pair.Key, "dog");
                        Assert.AreEqual(pair.Value, "biscuit");
                        break;
                    case 2:
                        Assert.AreEqual(pair.Key, "horse");
                        Assert.AreEqual(pair.Value, "grass");
                        break;
                    case 3:
                        Assert.AreEqual(pair.Key, "rabbit");
                        Assert.AreEqual(pair.Value, "carrot");
                        break;
                }
            }
        }

        [TestMethod]
        public void TestKeyOrder()
        {
            var dictionary = new OrderedDictionary<string, string>
            {
                {"monkey", "banana"},
                {"dog", "cat"},
                {"cat", "mouse"},
                {"horse", "grass"},
                {"rabbit", "carrot"}
            };
            int i = 0;
            foreach (var key in dictionary.Keys)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(key, "monkey");
                        break;
                    case 1:
                        Assert.AreEqual(key, "dog");
                        break;
                    case 2:
                        Assert.AreEqual(key, "cat");
                        break;
                    case 3:
                        Assert.AreEqual(key, "horse");
                        break;
                    case 4:
                        Assert.AreEqual(key, "rabbit");
                        break;
                }
            }
            dictionary["dog"] = "biscuit";
            // Order should not change
            i = 0;
            foreach (var key in dictionary.Keys)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(key, "monkey");
                        break;
                    case 1:
                        Assert.AreEqual(key, "dog");
                        break;
                    case 2:
                        Assert.AreEqual(key, "cat");
                        break;
                    case 3:
                        Assert.AreEqual(key, "horse");
                        break;
                    case 4:
                        Assert.AreEqual(key, "rabbit");
                        break;
                }
            }
            dictionary.Remove("cat");
            // everything moves accordingly
            i = 0;
            foreach (var key in dictionary.Keys)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(key, "monkey");
                        break;
                    case 1:
                        Assert.AreEqual(key, "dog");
                        break;
                    case 2:
                        Assert.AreEqual(key, "horse");
                        break;
                    case 3:
                        Assert.AreEqual(key, "rabbit");
                        break;
                }
            }
        }

        [TestMethod]
        public void TestValueOrder()
        {
            var dictionary = new OrderedDictionary<string, string>
            {
                {"monkey", "banana"},
                {"dog", "cat"},
                {"cat", "mouse"},
                {"horse", "grass"},
                {"rabbit", "carrot"}
            };
            int i = 0;
            foreach (var value in dictionary.Values)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(value, "banana");
                        break;
                    case 1:
                        Assert.AreEqual(value, "cat");
                        break;
                    case 2:
                        Assert.AreEqual(value, "mouse");
                        break;
                    case 3:
                        Assert.AreEqual(value, "grass");
                        break;
                    case 4:
                        Assert.AreEqual(value, "carrot");
                        break;
                }
            }
            dictionary["dog"] = "biscuit";
            // Order should not change
            i = 0;
            foreach (var value in dictionary.Values)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(value, "banana");
                        break;
                    case 1:
                        Assert.AreEqual(value, "biscuit");
                        break;
                    case 2:
                        Assert.AreEqual(value, "mouse");
                        break;
                    case 3:
                        Assert.AreEqual(value, "grass");
                        break;
                    case 4:
                        Assert.AreEqual(value, "carrot");
                        break;
                }
            }
            dictionary.Remove("cat");
            // everything moves accordingly
            i = 0;
            foreach (var value in dictionary.Values)
            {
                switch (i++)
                {
                    case 0:
                        Assert.AreEqual(value, "banana");
                        break;
                    case 1:
                        Assert.AreEqual(value, "biscuit");
                        break;
                    case 2:
                        Assert.AreEqual(value, "grass");
                        break;
                    case 3:
                        Assert.AreEqual(value, "carrot");
                        break;
                }
            }
        }

        [TestMethod]
        public void TestConsistence()
        {
            var dictionary = new OrderedDictionary<string, string> {{"monkey", "banana"}};
            dictionary["horse"] = "grass";
            Assert.IsTrue(dictionary.ContainsKey("monkey") && dictionary.ContainsKey("horse"));
            bool found1 = false, found2 = false;
            foreach (var pair in dictionary)
            {
                if ("monkey".Equals(pair.Key))
                {
                    found1 = true;
                }
                else if ("horse".Equals(pair.Key))
                {
                    found2 = true;
                }
            }
            Assert.IsTrue(found1);
            Assert.IsTrue(found2);
        }

        [TestMethod]
        public void TestSerialization()
        {
            var dictionary = new OrderedDictionary<string, string>(new CustomEqualityComparer()) { { "monkey", "banana" } };
            dictionary["horse"] = "grass";
            var dictionary2 = dictionary.Serialize().Deserialize<OrderedDictionary<string, string>>();
            Assert.IsTrue(dictionary.SequenceEqual(dictionary2));
        }

        [Serializable]
        private class CustomEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x == y;
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}