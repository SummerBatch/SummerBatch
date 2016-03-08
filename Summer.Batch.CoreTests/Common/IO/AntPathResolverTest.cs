using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;

namespace Summer.Batch.CoreTests.Common.IO
{
    [TestClass]
    public class AntPathResolverTest
    {
        private readonly AntPathResolver _antPathResolver = new AntPathResolver();

        [TestMethod]
        public void TestFindMatchingResources1()
        {
            var resources = _antPathResolver.FindMatchingResources("TestData/Sort/Input/sort*.txt");
            Assert.AreEqual(19, resources.Count());
        }

        [TestMethod]
        public void TestFindMatchingResources2()
        {
            var resources = _antPathResolver.FindMatchingResources("TestData/**/Input/sort*.txt");
            Assert.AreEqual(19, resources.Count());
        }
    }
}