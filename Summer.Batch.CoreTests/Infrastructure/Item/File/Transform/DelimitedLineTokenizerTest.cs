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
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Infrastructure.Item.File.Transform
{
    [TestClass]
    public class DelimitedLineTokenizerTest
    {
        private readonly DelimitedLineTokenizer _tokenizer = new DelimitedLineTokenizer();

        [TestMethod]
        public void TestTokenize1()
        {
            var fieldSet = _tokenizer.Tokenize("a, b,c");

            Assert.IsNotNull(fieldSet);
            Assert.AreEqual("a", fieldSet.Values[0]);
            Assert.AreEqual(" b", fieldSet.Values[1]);
            Assert.AreEqual("c", fieldSet.Values[2]);
        }

        [TestMethod]
        public void TestTokenize2()
        {
            var fieldSet = _tokenizer.Tokenize("a,b,c,\"a,b\",d");

            Assert.IsNotNull(fieldSet);
            Assert.AreEqual(5, fieldSet.Count);
            Assert.AreEqual("a", fieldSet.Values[0]);
            Assert.AreEqual("b", fieldSet.Values[1]);
            Assert.AreEqual("c", fieldSet.Values[2]);
            Assert.AreEqual("a,b", fieldSet.Values[3]);
            Assert.AreEqual("d", fieldSet.Values[4]);
        }
    }
}