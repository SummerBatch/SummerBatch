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
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.StepScope;
using Summer.Batch.Common.Proxy;

// ReSharper disable SuspiciousTypeConversion.Global
namespace Summer.Batch.CoreTests.StepScope
{
    [TestClass]
    public class StepScopeTest
    {
        private JobInstance _jobInstance;
        private JobExecution _jobExecution;
        private StepExecution _stepExecution;

        [TestInitialize]
        public void Initialize()
        {
            _jobInstance = new JobInstance(1, "testJob");
            _jobExecution = new JobExecution(_jobInstance, new JobParameters());
            _stepExecution = new StepExecution("testStep1", _jobExecution, 1);
        }

        [TestMethod]
        public void TestStepScopeConstructor1()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(new ResolvedParameter<B>("i")));
            container.RegisterStepScope<B>("i");

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeConstructor2()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(new ResolvedParameter<B>("i")));
            container.RegisterStepScope<I, B>("i");

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeConstructor3()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(new ResolvedParameter<B>()));
            container.RegisterStepScope<B>();

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeProperty1()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(), new InjectionProperty("I", new ResolvedParameter<B>("i")));
            container.RegisterStepScope<B>("i");

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeProperty2()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(), new InjectionProperty("I", new ResolvedParameter<B>("i")));
            container.RegisterStepScope<I, B>("i");

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeProperty3()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(), new InjectionProperty("I", new ResolvedParameter<B>()));
            container.RegisterStepScope<B>();

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeMethod1()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(), new InjectionMethod("SetI", new ResolvedParameter<B>("i")));
            container.RegisterStepScope<B>("i");

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeMethod2()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(), new InjectionMethod("SetI", new ResolvedParameter<B>("i")));
            container.RegisterStepScope<I, B>("i");

            Check(container);
        }

        [TestMethod]
        public void TestStepScopeMethod3()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(), new InjectionMethod("SetI", new ResolvedParameter<B>()));
            container.RegisterStepScope<B>();

            Check(container);
        }

        [TestMethod]
        public void TestAllInStepScope()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterStepScope<A>("a", new InjectionConstructor(new ResolvedParameter<B>("i")));
            container.RegisterStepScope<B>("i");

            StepSynchronizationManager.Register(_stepExecution);

            var a = container.Resolve<A>("a");

            Assert.IsNotNull(a.I);
            Assert.IsFalse(a.I is IProxyObject);
        }

        [TestMethod]
        public void TestStepScopeAdded()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(new ResolvedParameter<B>("i")));
            container.RegisterSingleton<I, B>("i");
            container.RegisterStepScope<B>("i");

            var a = container.Resolve<A>("a");

            Assert.IsNotNull(a.I);
            Assert.IsTrue(a.I is IProxyObject);
        }

        [TestMethod]
        public void TestStepScopeRemoved()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(new ResolvedParameter<B>("i")));
            container.RegisterStepScope<B>("i");
            container.RegisterSingleton<I, B>("i");

            var a = container.Resolve<A>("a");

            Assert.IsNotNull(a.I);
            Assert.IsFalse(a.I is IProxyObject);
        }

        [TestMethod]
        public void TestStepScopeUnchanged()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<StepScopeExtension>();
            container.RegisterSingleton<A>("a", new InjectionConstructor(new ResolvedParameter<B>("i")));
            container.RegisterStepScope<B>("i");
            container.RegisterType<I, B>("i");

            var a = container.Resolve<A>("a");

            Assert.IsNotNull(a.I);
            Assert.IsTrue(a.I is IProxyObject);
        }

        private void Check(IUnityContainer container)
        {
            var a = container.Resolve<A>("a");

            Assert.IsNotNull(a);
            Assert.IsNotNull(a.I);
            Assert.IsTrue(a.I is IProxyObject);

            StepSynchronizationManager.Register(_stepExecution);

            Assert.IsNotNull(((IProxyObject)a.I).GetInstance());
            Assert.AreEqual("B", a.M());
        }

        public class A
        {
            public A()
            {
            }

            public A(I i)
            {
                I = i;
            }

            public I I { get; set; }

            public void SetI(I i)
            {
                I = i;
            }

            public string M()
            {
                return I.M();
            }
        }

        public interface I
        {
            string M();
        }

        public class B : I
        {
            public string M()
            {
                return "B";
            }
        }
    }
}