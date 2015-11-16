using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;

namespace Summer.Batch.CoreTests.Common.IO
{
    [TestClass]
    public class ResourceLoaderTest
    {
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        [TestMethod]
        public void TestGetResources1()
        {
            var resources = _resourceLoader.GetResources("TestData/Sort/Input/sort*.txt");
            Assert.AreEqual(18, resources.Count);
        }

        [TestMethod]
        public void TestGetResources2()
        {
            var resources = _resourceLoader.GetResources(@"file://TestData\Sort\Input\sort*.txt");
            Assert.AreEqual(18, resources.Count);
        }
    }
}