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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Infrastructure.Item.File.Transform
{
    [TestClass]
    public class PassThroughFieldExtractorTest
    {
        private readonly PassThroughFieldExtractor _extractor = new PassThroughFieldExtractor();

        [TestMethod]
        public void TestExtractDictionary()
        {
            var dictionary = new Dictionary<string, string> { { "key", "value" } };

            var result = _extractor.Extract(dictionary);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("value", result[0]);
        }

        [TestMethod]
        public void TestExtractArray()
        {
            var array = new[] { "string" };

            var result = _extractor.Extract(array);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("string", result[0]);
        }

        [TestMethod]
        public void TestExtractCollection()
        {
            ICollection<string> collection = new List<string> { "string" };

            var result = _extractor.Extract(collection);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("string", result[0]);
        }

        [TestMethod]
        public void TestExtractFieldSet()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "value" });

            var result = _extractor.Extract(fieldSet);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("value", result[0]);
        }

        [TestMethod]
        public void TestExtractObject()
        {
            var obj = new object();

            var result = _extractor.Extract(obj);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(obj, result[0]);
        }
    }
}