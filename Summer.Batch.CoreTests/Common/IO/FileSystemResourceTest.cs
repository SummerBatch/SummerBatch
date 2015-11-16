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
using Summer.Batch.Common.IO;
using System;
using System.IO;

namespace Summer.Batch.CoreTests.Common.IO
{
    [TestClass]
    public class FileSystemResourceTest
    {
        private const string TestDataDirectory = @"..\..\TestData";

        private readonly string _testPath = Path.Combine(TestDataDirectory, @"FlatFile\Test.txt");
        private readonly string _testDataFullPath = new Uri(Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), TestDataDirectory)).LocalPath.Replace('/','\\');

        private IResource _resource;

        [TestInitialize]
        public void Initialize()
        {
            _resource = new FileSystemResource(_testPath);
        }

        [TestMethod]
        public void TestExists1()
        {
            Assert.IsTrue(_resource.Exists());
        }

        [TestMethod]
        public void TestExists2()
        {
            var resource = new FileSystemResource(Path.Combine(TestDataDirectory, "NonExistingFile"));
            Assert.IsFalse(resource.Exists());
        }

        [TestMethod]
        public void TestGetInputStream()
        {
            var stream = _resource.GetInputStream();

            Assert.IsNotNull(stream);
        }

        [TestMethod]
        public void TestGetUri()
        {
            var uri = _resource.GetUri();

            var expected = new Uri(Path.Combine(_testDataFullPath, @"FlatFile\Test.txt"));
            Assert.AreEqual(expected, uri);
        }

        [TestMethod]
        public void TestGetFileInfo1()
        {
            var fileInfo = _resource.GetFileInfo();

            Assert.IsNotNull(fileInfo);
            Assert.AreEqual("Test.txt", fileInfo.Name);
        }

        [TestMethod]
        public void TestGetFileInfo2()
        {
            var resource = new FileSystemResource(TestDataDirectory);
            var fileInfo = resource.GetFileInfo();

            Assert.IsNotNull(fileInfo);
            Assert.AreEqual("TestData", fileInfo.Name);
        }

        [TestMethod]
        public void TestGetLastModified()
        {
            var now = DateTime.Now;
            File.SetLastWriteTime(_testPath, now);

            var lastModified = _resource.GetLastModified();

            Assert.AreEqual(now, lastModified);
        }

        [TestMethod]
        public void TestGetFilename()
        {
            Assert.AreEqual("Test.txt", _resource.GetFilename());
        }

        [TestMethod]
        public void TestGestDescription()
        {
            Assert.AreEqual("File["+_testDataFullPath+@"\FlatFile\Test.txt]", _resource.GetDescription());
        }

        [TestMethod]
        public void TestCreateRelative()
        {
            var resource2 = _resource.CreateRelative(@"..\Test2.txt");

            Assert.AreEqual(_testDataFullPath+@"\FlatFile\Test2.txt", resource2.GetFileInfo().FullName);
        }

        [TestMethod]
        public void TestDirectory()
        {
            var testDir = new FileSystemResource(TestDataDirectory);
            Assert.IsTrue(testDir.GetFileInfo().Attributes.HasFlag(FileAttributes.Directory));
            Assert.IsFalse(testDir.GetFileInfo().Attributes.HasFlag(FileAttributes.Normal));
        }
    }
}