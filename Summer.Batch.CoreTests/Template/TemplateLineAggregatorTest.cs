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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Extra.Template;
using Summer.Batch.Common.IO;

namespace Summer.Batch.CoreTests.Template
{
    [TestClass]
    public class TemplateLineAggregatorTest
    {
        private readonly IResource _template = new FileSystemResource(@"TestData\Template\Template.txt");

        private readonly Person[] _persons = { new Person(1, "Person1"), new Person(2, "Person2") };

        [TestMethod]
        public void TestAggregate1()
        {
            var expected = " -----------------------" + Environment.NewLine + "!   1 ! Person1         !" + Environment.NewLine +
                           " -----------------------" + Environment.NewLine + "!   2 ! Person2         !" + Environment.NewLine;

            var aggregator = new PersonAggregator
            {
                Template = _template,
                TemplateId = "person1"
            };
            aggregator.AfterPropertiesSet();

            var sb = new StringBuilder();
            foreach (var person in _persons)
            {
                sb.Append(aggregator.Aggregate(person));
                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());

            Assert.AreEqual(expected, sb.ToString());
        }

        [TestMethod]
        public void TestAggregate2()
        {
            var expected = " -----------------------" + Environment.NewLine + "! 001 ! Person1         !" + Environment.NewLine +
                           " -----------------------" + Environment.NewLine + "! 002 ! Person2         !" + Environment.NewLine;

            var aggregator = new PersonAggregator
            {
                Template = _template,
                TemplateId = "person2"
            };
            aggregator.AfterPropertiesSet();

            var sb = new StringBuilder();
            foreach (var person in _persons)
            {
                sb.Append(aggregator.Aggregate(person));
                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());

            Assert.AreEqual(expected, sb.ToString());
        }
    }

    struct Person
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public Person(int id, string name)
            : this()
        {
            Id = id;
            Name = name;
        }
    }

    class PersonAggregator : AbstractTemplateLineAggregator<Person>
    {
        protected override IEnumerable<object> GetParameters(Person person)
        {
            return new object[] { person.Id, person.Name };
        }
    }
}