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
using Summer.Batch.Core;
using Summer.Batch.Core.Listener;
using Summer.Batch.Common.Util;
using System.Collections.Generic;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Core.Listener
{
    [TestClass()]
    public class OrderedCompositeTests
    {
        [TestMethod()]
        public void SetItemsTest()
        {
            List<IStepListener> list = new List<IStepListener>
            {
                new SecondListener(),
                new UnorderedListener(),
                new FirstListener(),
                new ThirdListener()
            };
            OrderedComposite<IStepListener> listeners = new OrderedComposite<IStepListener>();
            listeners.SetItems(list);
            Assert.IsNotNull(listeners);
            IEnumerator<IStepListener> enumerator = listeners.Enumerator();
            Assert.IsNotNull(enumerator);
            int i = 1;
            //Test order is being well understood
            while (enumerator.MoveNext())
            {
                object item = enumerator.Current;
                switch (i)
                {
                    case 1: Assert.IsTrue(item is FirstListener); break;
                    case 2: Assert.IsTrue(item is SecondListener); break;
                    case 3: Assert.IsTrue(item is ThirdListener); break;
                    case 4: Assert.IsTrue(item is UnorderedListener); break;
                }
                i++;
            }
        }

        [TestMethod()]
        public void AddTest()
        {
            OrderedComposite<IStepListener> listeners = new OrderedComposite<IStepListener>();
            listeners.Add(new UnorderedListener());
            Assert.IsNotNull(listeners);
            listeners.Add(new ThirdListener());
            Assert.IsNotNull(listeners);
            listeners.Add(new FirstListener());
            Assert.IsNotNull(listeners);
            listeners.Add(new SecondListener());
            Assert.IsNotNull(listeners);

        }

        [TestMethod()]
        public void EnumeratorTest()
        {
            var listeners = TearUp();
            IEnumerator<IStepListener> enumerator = listeners.Enumerator();
            Assert.IsNotNull(enumerator);
            int i = 1;
            while (enumerator.MoveNext())
            {
                object item = enumerator.Current;
                switch (i)
                {
                    case 1 :Assert.IsTrue(item is FirstListener);Assert.AreEqual(1,OrderHelper.GetOrderFromAttribute(item).Value); break;
                    case 2: Assert.IsTrue(item is SecondListener); Assert.AreEqual(2,OrderHelper.GetOrderFromAttribute(item).Value); break;
                    case 3: Assert.IsTrue(item is ThirdListener); Assert.AreEqual(3,OrderHelper.GetOrderFromAttribute(item).Value); break;
                    case 4: Assert.IsTrue(item is UnorderedListener); Assert.IsNull(OrderHelper.GetOrderFromAttribute(item)); break;
                }
                i++;
            }
        }

        private static OrderedComposite<IStepListener> TearUp()
        {
            OrderedComposite<IStepListener> listeners = new OrderedComposite<IStepListener>();
            listeners.Add(new UnorderedListener());
            listeners.Add(new ThirdListener());
            listeners.Add(new FirstListener());
            listeners.Add(new SecondListener());
            return listeners;
        }

        [TestMethod()]
        public void ReverseTest()
        {
            var listeners = TearUp();
            IEnumerator<IStepListener> enumerator = listeners.Reverse();
            Assert.IsNotNull(enumerator);
            int i = 1;
            while (enumerator.MoveNext())
            {
                object item = enumerator.Current;
                switch (i)
                {
                    case 4: Assert.IsTrue(item is FirstListener); break;
                    case 3: Assert.IsTrue(item is SecondListener); break;
                    case 2: Assert.IsTrue(item is ThirdListener); break;
                    case 1: Assert.IsTrue(item is UnorderedListener); break;
                }
                i++;
            }
        }


        #region FirstListener
        [Order(1)]
        class FirstListener : IStepListener
        {
        }
        #endregion

        #region SecondListener
        [Order(2)]
        class SecondListener : IStepListener
        {
        }
        #endregion

        #region ThirdListener
        [Order(3)]
        class ThirdListener : IStepListener
        {
        }
        #endregion

        #region UnorderedListener
        class UnorderedListener : IStepListener
        {
        }
        #endregion
    }
}
