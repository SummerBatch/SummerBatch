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

namespace Summer.Batch.CoreTests.Common.Collections
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void TestCapacity1()
        {
            var queue = new PriorityQueue<string>();

            Assert.AreEqual(0, queue.Capacity);
        }

        [TestMethod]
        public void TestCapacity2()
        {
            var queue = new PriorityQueue<string>(33);

            Assert.AreEqual(33, queue.Capacity);
        }

        [TestMethod]
        public void TestCapacity3()
        {
            var queue = new PriorityQueue<string> {Capacity = 27};


            Assert.AreEqual(27, queue.Capacity);
        }

        [TestMethod]
        public void TestCapacity4()
        {
            var queue = new PriorityQueue<string>(25) {Capacity = 0};


            Assert.AreEqual(0, queue.Capacity);
        }

        [TestMethod]
        public void TestCapacity5()
        {
            var queue = new PriorityQueue<string>(new[] { "string" });

            Assert.AreEqual(1, queue.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCapacity6()
        {
            var queue = new PriorityQueue<string>(new[] {"string"}) {Capacity = 0};

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCapacity7()
        {
            new PriorityQueue<string>(-1);
        }

        [TestMethod]
        public void TestCapacity8()
        {
            var queue = new PriorityQueue<string>(0);

            Assert.AreEqual(0, queue.Capacity);
        }

        [TestMethod]
        public void TestCapacity9()
        {
            var queue = new PriorityQueue<string>(Enumerable.Range(0, 10).Select(x => x.ToString()));

            Assert.AreEqual(16, queue.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor()
        {
            new PriorityQueue<string>((IEnumerable<string>)null);
        }

        [TestMethod]
        public void TestIsReadOnly()
        {
            var queue = new PriorityQueue<string>();

            Assert.IsFalse(queue.IsReadOnly);
        }

        [TestMethod]
        public void TestPoll1()
        {
            var queue = new PriorityQueue<string> { "string", "anotherString" };

            var poll = queue.Poll();

            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual("anotherString", poll);
            Assert.IsFalse(queue.Contains("anotherString"));
        }

        [TestMethod]
        public void TestPoll2()
        {
            var queue = new PriorityQueue<string>();

            var poll = queue.Poll();

            Assert.IsNull(poll);
        }

        [TestMethod]
        public void TestPeek1()
        {
            var queue = new PriorityQueue<string> { "string", "anotherString" };

            var poll = queue.Peek();

            Assert.AreEqual(2, queue.Count);
            Assert.AreEqual("anotherString", poll);
            Assert.IsTrue(queue.Contains("anotherString"));
        }

        [TestMethod]
        public void TestPeek2()
        {
            var queue = new PriorityQueue<string>();

            var poll = queue.Peek();

            Assert.IsNull(poll);
        }

        [TestMethod]
        public void TestClear()
        {
            var queue = new PriorityQueue<string> { "string " };

            queue.Clear();

            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void TestCopyTo()
        {
            var queue = new PriorityQueue<string> { "string", "anotherString" };
            var array = new string[3];

            queue.CopyTo(array, 1);

            Assert.IsNull(array[0]);
            Assert.AreEqual("anotherString", array[1]);
            Assert.AreEqual("string", array[2]);
        }

        [TestMethod]
        public void TestRemove1()
        {
            var queue = new PriorityQueue<string> { "string", "anotherString" };

            var result = queue.Remove("string");

            Assert.IsTrue(result);
            Assert.AreEqual(1, queue.Count);
            Assert.IsFalse(queue.Contains("string"));
        }

        [TestMethod]
        public void TestRemove2()
        {
            var queue = new PriorityQueue<string> { "string", "anotherString" };

            var result = queue.Remove("someString");

            Assert.IsFalse(result);
            Assert.AreEqual(2, queue.Count);
        }

        [TestMethod]
        public void TestComparer1()
        {
            var comparer = Comparer<string>.Create((x, y) => string.Compare(y, x, StringComparison.Ordinal));
            var queue = new PriorityQueue<string>(comparer) { "string", "anotherString" };
            var expected = new[] { "string", "anotherString" };

            Assert.IsTrue(queue.SequenceEqual(expected));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestComparer2()
        {
            new PriorityQueue<object>();
        }
    }
}