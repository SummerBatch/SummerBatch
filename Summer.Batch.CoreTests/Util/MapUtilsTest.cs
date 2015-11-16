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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Extra.Utils;
using System.Collections.Generic;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public class MapUtilsTest
    {
        #region Test ContainsKey, ContainsValue.

        ///<summary>
        /// MapUtils.ContainsKey method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_ContainsKey1() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            bool result = MapUtils.ContainsKey(dictionary, null);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        ///<summary>
        /// MapUtils.ContainsKey method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_ContainsKey2() {
            IDictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            dictionary.Add("3", 3);
            bool result = MapUtils.ContainsKey(dictionary, "2");
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        ///<summary>
        /// MapUtils.ContainsValue method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_ContainsValue1() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            bool result = MapUtils.ContainsValue(dictionary, null);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        ///<summary>
        /// MapUtils.ContainsValue method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_ContainsValue2() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            dictionary.Add("3", 3);
            bool result = MapUtils.ContainsValue(dictionary, 2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        #endregion

        #region Test KeySet.

        ///<summary>
        /// MapUtils.KeySet method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_KeySet1() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            ISet<object> result = MapUtils.KeySet(dictionary);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    
        ///<summary>
        /// MapUtils.KeySet method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_KeySet2() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            dictionary.Add("3", 3);
            ISet<object> result = MapUtils.KeySet(dictionary);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        #endregion

        #region Test Put, PutAll.

        ///<summary>
        /// MapUtils.Put method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Put1() {
            Dictionary<object, object> dictionary1 = new Dictionary<object, object>();
            MapUtils.Put(dictionary1,"1",1);
            MapUtils.Put(dictionary1,"2",2);
            MapUtils.Put(dictionary1,"3",null);
            Assert.IsTrue(dictionary1.ContainsKey("1"));
            Assert.IsTrue(dictionary1.ContainsKey("2"));
            Assert.IsTrue(dictionary1.ContainsKey("3"));
            Assert.IsTrue(dictionary1.ContainsValue(1));
            Assert.IsTrue(dictionary1.ContainsValue(2));
            Assert.IsTrue(dictionary1.ContainsValue(null));
        }

        ///<summary>
        /// MapUtils.PutAll method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_PutAll1() {
            Dictionary<object, object> dictionary1 = new Dictionary<object, object>();
            dictionary1.Add("1", 1);
            dictionary1.Add("2", 2);
            dictionary1.Add("3", 3);

            Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
            dictionary2.Add("4", 4);
            dictionary2.Add("5", 5);

            MapUtils.PutAll(dictionary1, dictionary2);
            Assert.AreEqual(5, dictionary1.Count);

        }

        #endregion

        #region Test Remove.

        ///<summary>
        /// MapUtils.Remove method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Remove1() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            object result = MapUtils.Remove(dictionary, null);
            Assert.AreEqual(null, result);
        }

        ///<summary>
        /// MapUtils.Remove method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Remove2() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            object result = MapUtils.Remove(dictionary, "1");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        #endregion

        #region Test Get.

        ///<summary>
        /// MapUtils.Get method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Get1() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            object result = MapUtils.Get(dictionary, null);
            Assert.AreEqual(null, result);
        }

        ///<summary>
        /// MapUtils.Get method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Get2() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            object result = MapUtils.Get(dictionary, "1");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        #endregion

        #region Test Values.

        ///<summary>
        /// MapUtils.Values method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Values1() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            ICollection<object> result = MapUtils.Values(dictionary);
            Assert.IsTrue(result.Count==0);
        }

        ///<summary>
        /// MapUtils.Values method test.
        ///</summary>
        [TestMethod]
        public void MapUtils_Values2() {
            Dictionary<object, object> dictionary = new Dictionary<object,object>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            dictionary.Add("3", 3);
            ICollection<object> result = MapUtils.Values(dictionary);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            var enumerator = result.GetEnumerator();
            enumerator.MoveNext();
            Assert.AreEqual(1, enumerator.Current);
        }

        #endregion
    }
}
