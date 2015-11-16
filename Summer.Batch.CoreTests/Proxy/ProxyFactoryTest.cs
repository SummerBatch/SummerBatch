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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.Proxy;

namespace Summer.Batch.CoreTests.Proxy
{
    [TestClass]
    public class ProxyFactoryTest
    {
        [TestMethod]
        public void TestProperty()
        {
            var dummy = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy);

            proxy.StringProperty = "test";
            var result = proxy.StringProperty;

            Assert.AreEqual("test", result);
            Assert.AreEqual("test", ((IDummy)dummy).StringProperty);
        }

        [TestMethod]
        public void TestPropertyCollision()
        {
            // Checks that there are no collision between
            // IProxyObject<T>.Instance and other properties
            var dummy = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy);

            proxy.Instance = "test";
            var result = proxy.Instance;

            Assert.AreEqual("test", result);
            Assert.AreSame(dummy, ((IProxyObject)proxy).GetInstance());
        }

        [TestMethod]
        public void TestChangeInstance()
        {
            var dummy1 = new Dummy();
            var dummy2 = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy1);

            proxy.StringProperty = "dummy1";
            ((IProxyObject)proxy).SetInstance(dummy2);
            proxy.StringProperty = "dummy2";

            Assert.AreEqual("dummy1", ((IDummy)dummy1).StringProperty);
            Assert.AreEqual("dummy2", ((IDummy)dummy2).StringProperty);
        }

        [TestMethod]
        public void TestExplicitImplementations()
        {
            var dummy1 = new Dummy();
            var dummy2 = new Dummy();
            ((IDummy)dummy1).StringProperty = "a";
            ((IDummy)dummy2).StringProperty = "b";
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy1);

            var result1 = proxy.CompareTo(dummy2);
            var result2 = ((IComparable<IDummy>)proxy).CompareTo(dummy2);

            Assert.IsTrue(result1 < 0);
            Assert.AreEqual(-result2, result1);
        }

        [TestMethod]
        public void TestMethods()
        {
            var dummy = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy);

            var result1 = proxy.M();
            var result2 = proxy.M(true);
            var result3 = proxy.M("string");
            var result4 = proxy.M((object)"string");

            Assert.AreEqual("void", result1);
            Assert.AreEqual("object: True", result2);
            Assert.AreEqual("string", result3);
            Assert.AreEqual("object: string", result4);
        }

        [TestMethod]
        public void TestEvent1()
        {
            var dummy = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy);
            var executed = false;

            proxy.Event += (sender, eventArgs) => executed = true;
            proxy.M();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void TestEvent2()
        {
            var dummy = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy);
            var executed = false;
            var eventHandler = new EventHandler((sender, eventArgs) => executed = true);

            proxy.Event += eventHandler;
            proxy.Event -= eventHandler;
            proxy.M();

            Assert.IsFalse(executed);
        }

        [TestMethod]
        public void TestProxyTypeCache()
        {
            var dummy = new Dummy();
            var proxy1 = ProxyFactory.Create<IDummy>(instance: dummy);
            var proxy2 = ProxyFactory.Create<IDummy>(instance: dummy);

            Assert.AreSame(proxy1.GetType(), proxy2.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(ProxyException))]
        public void TestNullInstance()
        {
            var proxy = ProxyFactory.Create<IDummy>();

            Assert.IsNotNull(proxy);

            proxy.M();
        }

        [TestMethod]
        public void TestIndex()
        {
            var dummy = new Dummy();
            var proxy = ProxyFactory.Create<IDummy>(instance: dummy);

            proxy[3] = "three";
            var result = proxy[3];

            Assert.AreEqual("three", result);
        }

        public interface IDummy : IComparable<IDummy>
        {
            event EventHandler Event;

            string StringProperty { get; set; }

            object Instance { get; set; }

            string M();

            string M(object obj);

            string M(string str);

            new int CompareTo(IDummy other);

            string this[int index] { get; set; }
        }

        public sealed class Dummy : IDummy
        {
            public event EventHandler Event;

            string IDummy.StringProperty { get; set; }

            public object Instance { get; set; }

            public string M()
            {
                return M("void");
            }

            public string M(object obj)
            {
                return M("object: " + obj);
            }

            public string M(string str)
            {
                if (Event != null) Event(this, new EventArgs());
                return str;
            }

            private int CompareTo(IDummy other)
            {
                return string.Compare(((IDummy)this).StringProperty, other.StringProperty, StringComparison.Ordinal);
            }

            int IComparable<IDummy>.CompareTo(IDummy other)
            {
                return -CompareTo(other);
            }

            int IDummy.CompareTo(IDummy other)
            {
                return CompareTo(other);
            }

            private readonly IDictionary<int, string> _dictionary = new Dictionary<int, string>();

            public string this[int index]
            {
                get
                {
                    string result;
                    return _dictionary.TryGetValue(index, out result) ? result : null;
                }
                set
                {
                    _dictionary[index] = value;
                }
            }
        }
    }
}