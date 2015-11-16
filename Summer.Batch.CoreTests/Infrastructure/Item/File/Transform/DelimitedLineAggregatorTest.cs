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

namespace Summer.Batch.CoreTests.Infrastructure.Item.File.Transform
{
    [TestClass]
    public class DelimitedLineAggregatorTest
    {
        [TestMethod]
        public void TestAggregate1()
        {
            var aggregator = new DelimitedLineAggregator<Person>
            {
                FieldExtractor = new PropertyFieldExtractor<Person> { Names = new[] { "Id", "Name" } }
            };
            var person = new Person { Id = 1, Name = "Person 1" };

            var result = aggregator.Aggregate(person);

            Assert.AreEqual("1,Person 1", result);
        }

        [TestMethod]
        public void TestAggregate2()
        {
            var aggregator = new DelimitedLineAggregator<Person>
            {
                FieldExtractor = new PropertyFieldExtractor<Person> { Names = new[] { "Id", "Name" } },
                Delimiter = ";"
            };
            var person = new Person { Id = 1, Name = "Person 1" };

            var result = aggregator.Aggregate(person);

            Assert.AreEqual("1;Person 1", result);
        }
    }
}