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
using Summer.Batch.Common.Property;

namespace Summer.Batch.CoreTests.Infrastructure.Item.File.Transform
{
    [TestClass]
    public class PropertyFieldExtractorTest
    {
        [TestMethod]
        public void TestExtract1()
        {
            var extractor = new PropertyFieldExtractor<Person> { Names = new[] { "Id", "Name", "Parent.Parent.Name" } };
            var person = new Person { Id = 1, Name = "Person1" };

            var result = extractor.Extract(person);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1L, result[0]);
            Assert.AreEqual("Person1", result[1]);
            Assert.IsNull(result[2]);
        }

        [TestMethod]
        public void TestExtract2()
        {
            var extractor = new PropertyFieldExtractor<Person> { Names = new[] { "Id", "Name", "Parent.Parent.Name" } };
            var person = new Person
            {
                Id = 3,
                Name = "Person3",
                Parent = new Person
                {
                    Id = 2,
                    Name = "Person2",
                    Parent = new Person
                    {
                        Id = 1,
                        Name = "Person1"
                    }
                }
            };

            var result = extractor.Extract(person);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(3L, result[0]);
            Assert.AreEqual("Person3", result[1]);
            Assert.AreEqual("Person1", result[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPropertyException))]
        public void TestExtract3()
        {
            var extractor = new PropertyFieldExtractor<Person> { Names = new[] { "Id", "Name", "InvalidProperty" } };
            var person = new Person { Id = 1, Name = "Person1" };

            extractor.Extract(person);
        }
    }

    class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Person Parent { get; set; }

        public override string ToString()
        {
            return string.Format("[Person-Id({0}),Name({1})", Id, Name);
        }
    }
}