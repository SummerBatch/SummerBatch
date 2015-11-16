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
    public class FixedLengthTokenizerTest
    {
        [TestMethod]
        public void TestTokenize1()
        {
            var tokenizer = new FixedLengthTokenizer
            {
                Columns = new[] { new Range(1, 2), new Range(3, 10) }
            };

            var result = tokenizer.Tokenize(" 1Person 1");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(" 1", result.ReadRawString(0));
            Assert.AreEqual("Person 1", result.ReadRawString(1));
        }

        [TestMethod]
        public void TestTokenize2()
        {
            var tokenizer = new FixedLengthTokenizer
            {
                Columns = new[] { new Range(1, 2), new Range(3) }
            };

            var result = tokenizer.Tokenize(" 1Person 1     ");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(" 1", result.ReadRawString(0));
            Assert.AreEqual("Person 1     ", result.ReadRawString(1));
        }
    }
}