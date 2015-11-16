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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Extra.Sort.Legacy.Parser;

namespace Summer.Batch.CoreTests.Sort
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestLexer1()
        {
            string[] expected = { "(", "1", "25", "A", "26", "10", "ZD", "D", ")" };
            var lexer = new Lexer("(1,25,A,26,10,ZD,D)");
            var results = new List<string>();
            while (lexer.MoveNext())
            {
                results.Add(lexer.Current);
            }
            Assert.IsTrue(results.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestLexer2()
        {
            string[] expected = { "(", "1", "25", "A", "26", "10", "ZD", "D", ")" };
            var lexer = new Lexer("(1, 25, A, 26, 10, ZD , D )");
            var results = new List<string>();
            while (lexer.MoveNext())
            {
                results.Add(lexer.Current);
            }
            Assert.IsTrue(results.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestLexer3()
        {
            string[] expected = { "1", "25", "A", "26", "10", "ZD", "D" };
            var lexer = new Lexer("1,25,A,26,10,ZD,D");
            var results = new List<string>();
            while (lexer.MoveNext())
            {
                results.Add(lexer.Current);
            }
            Assert.IsTrue(results.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestLexer4()
        {
            string[] expected = { "(", "C", "'srchfor ''", "1", "7", "C", "'''", "80:X", ")" };
            var lexer = new Lexer("(C'srchfor ''',1,7,C'''',80:X)");
            var results = new List<string>();
            while (lexer.MoveNext())
            {
                results.Add(lexer.Current);
            }
            Assert.IsTrue(results.SequenceEqual(expected));
        }
    }
}