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
using Summer.Batch.Infrastructure.Support;
using System.Collections.Specialized;

namespace Summer.Batch.CoreTests.Infrastructure.Support
{
    [TestClass]
    public class PropertiesConverterTest
    {
        [TestMethod]
        public void TestStringToProperties()
        {
            string s = "prop1=val1,prop2=val2";
            NameValueCollection props = PropertiesConverter.StringToProperties(s);
            Assert.AreEqual(props.Count, 2);
            Assert.AreEqual( "val1",props.Get("prop1"));
            Assert.AreEqual("val2",props.Get("prop2"));
        }

        [TestMethod]
        public void TestStringToPropertiesEmpty()
        {
            string s = "";
            NameValueCollection props = PropertiesConverter.StringToProperties(s);
            Assert.AreEqual(0,props.Count);
        }

        [TestMethod]
        public void TestPropertiesToString()
        {
            string s1 = "prop1=val1,prop2=val2";
            string s2 = "prop2=val2,prop1=val1";
            NameValueCollection props = new NameValueCollection {{"prop1", "val1"}, {"prop2", "val2"}};
            string result = PropertiesConverter.PropertiesToString(props);
            Assert.IsTrue(result.Equals(s1) || result.Equals(s2));
        }

        [TestMethod]
        public void TestPropertiesToStringEmpty()
        {
            NameValueCollection props = new NameValueCollection();
            Assert.AreEqual("",PropertiesConverter.PropertiesToString(props));
        }

        [TestMethod]
        public void TestPropertiesToStringId1()
        {
            NameValueCollection props = new NameValueCollection {{"aa", "bb"}, {"bb", "aa"}};
            NameValueCollection props2 = PropertiesConverter.StringToProperties(PropertiesConverter.PropertiesToString(props));
            Assert.AreEqual(2,props2.Count);
            Assert.AreEqual("bb",props2.Get("aa"));
            Assert.AreEqual("aa",props2.Get("bb"));
        }

        [TestMethod]
        public void TestPropertiesToStringId2()
        {
            // Warning: = in the key is not handled properly, = in the value is OK
            NameValueCollection props = new NameValueCollection {{"a=a", "bb"}, {"bb", "a=a"}};
            NameValueCollection props2 = PropertiesConverter.StringToProperties(PropertiesConverter.PropertiesToString(props));
            Assert.AreEqual(2,props2.Count);
            Assert.AreEqual( "a=bb",props2.Get("a"));
            Assert.AreEqual( "a=a",props2.Get("bb"));
        }
    }
}
