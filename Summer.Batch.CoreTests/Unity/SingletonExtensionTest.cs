using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core.Unity.Singleton;

namespace Summer.Batch.CoreTests.Unity
{
    [TestClass]
    public class SingletonExtensionTest
    {
        private IUnityContainer _container;

        [TestInitialize]
        public void Initialize()
        {
            _container = new UnityContainer();
            _container.AddNewExtension<SingletonExtension>();
        }

        [TestMethod]
        public void TestSingletonExtension1()
        {
            _container.RegisterType<object, A>("test");

            var result1 = _container.Resolve<object>("test");
            var result2 = _container.Resolve<object>("test");

            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void TestSingletonExtension2()
        {
            _container.RegisterType<object, List<A>>("test", new InjectionConstructor());

            var result1 = _container.Resolve<object>("test");
            var result2 = _container.Resolve<object>("test");

            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void TestSingletonExtension3()
        {
            var result1 = _container.Resolve<B>();
            var result2 = _container.Resolve<B>();

            Assert.AreSame(result1, result2);
            Assert.AreSame(result1.A, result2.A);
        }

        private class A { }

        private class B
        {
            [Dependency]
            public A A { get; set; }
        }
    }
}