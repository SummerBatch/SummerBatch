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
using Summer.Batch.CoreTests.Util.Test;
using Summer.Batch.Extra.Utils;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public sealed class ObjectUtilsExtraTests
    {

        #region testing classes.

        public class InnerObject
        {
            string name;
        
            public string getName() 
            {
                return name;
            }

            public InnerObject(string aName)
            {
                this.name = aName;
            }
        
            public override bool Equals(object obj) 
            {
                if(obj is InnerObject){
                    return this.name.Equals( ((InnerObject)obj).getName());
                } else {
                    return base.Equals(obj);
                }
            }

            public override string ToString()
            {
                return "Innerobject {"+this.name+"}";
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    
        public class Person
        {
            public string FirstName {get; set;}
            public string LastName {get; set;}
        
            public Person(string aFirstName, string aLastName)
            {
                FirstName = aFirstName;
                LastName = aLastName;
            }

            public Person()
            {
                FirstName ="";
                LastName ="";
            }
        }
    
        #endregion

        #region Test CopyProperties.

        ///<summary>
        /// ObjectUtils.copyProperties method test.
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testCopyProperties1() 
        {
            Person orig = new Person("Foo","Bar");
            Person dest = new Person();
            ObjectUtils.CopyProperties(dest, orig);
            Assert.IsNotNull(dest);
            Assert.IsNotNull(dest.FirstName);
            Assert.IsNotNull(dest.LastName);
            Assert.AreEqual("Foo", dest.FirstName);
            Assert.AreEqual("Bar", dest.LastName);
        }

        #endregion

        #region Test AreEqualTest.

        ///<summary>
        /// ObjectUtils.equals method test.
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testAreEqual1() {
            object obj1 = new object();
            object obj2 = new object();
            bool result = ObjectUtils.AreEqual(obj1, obj2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        ///<summary>
        /// ObjectUtils.equals method test.
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testAreEqual2() {
            InnerObject obj1 = new InnerObject("Foo");
            InnerObject obj2 = new InnerObject("Foo");
            bool result = ObjectUtils.AreEqual(obj1, obj2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        #endregion

        #region Test GetObjectByTypeTest.

        ///<summary>
        /// ObjectUtils.getObjectByType
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testGetObjectByType1() {
            object[] resultset = new object[] { new object() };
            string type = "";
            object result = ObjectUtils.GetObjectByType(resultset, type);
            Assert.AreEqual(null, result);
        }

        ///<summary>
        /// ObjectUtils.getObjectByType
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testGetObjectByType2() {
            object[] resultset = new object[] { new InnerObject("Foo"), new Person("Foo", "Bar") };
            string type = typeof(InnerObject).Name;
            object result = ObjectUtils.GetObjectByType(resultset, type);
            Assert.IsNotNull(result);
        }

        #endregion

        #region Test IsNull.

        ///<summary>
        /// ObjectUtils.isNull method test.
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testIsNull1() {
            object obj = null;
            bool result = ObjectUtils.IsNull(obj);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        ///<summary>
        /// ObjectUtils.isNull method test.
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testIsNull2() {
            object obj = new object();
            bool result = ObjectUtils.IsNull(obj);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        #endregion

        #region Test ToString

        ///<summary>
        /// ObjectUtils.toString method test.
        ///</summary>
        [TestMethod]
        public void ObjectUtils_testToString1() {
            InnerObject obj = new InnerObject("Foo");
            string result = ObjectUtils.ToString(obj);
            Assert.AreEqual("Innerobject {Foo}", result);
        }

        #endregion

    }
}
