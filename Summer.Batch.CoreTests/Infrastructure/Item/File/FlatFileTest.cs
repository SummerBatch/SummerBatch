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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Transactions;
using NLog;
using Summer.Batch.Infrastructure.Item.File.Transform;
using Summer.Batch.Common.Transaction;

namespace Summer.Batch.CoreTests.Infrastructure.Item.File
{
    [TestClass]
    public class FlatFileTest
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string TestDataDirectory = @"..\..\TestData";
        private const int CountObjects = 20;
        private const int CountTransactions = 3;
        private readonly string _testPath = Path.Combine(TestDataDirectory, @"FlatFile\Test.txt");
        private readonly string _testPath2 = Path.Combine(TestDataDirectory, @"FlatFile\TestTransactional.txt");
        private readonly string _testpath3 = "C:/temp/outputs/Test.txt";

        [TestMethod]
        public void FlatFileTestTransactionalTest()
        {
            var writer = new FlatFileItemWriter<Person>
            {
                Resource = new FileSystemResource(_testPath2),
                LineAggregator = new LineAggregator(),
                HeaderWriter = new HeaderWriter()
            };
            var reader = new FlatFileItemReader<Person>
            {
                Resource = new FileSystemResource(_testPath2),
                LinesToSkip = 2,
                LineMapper = new LineMapper()
            };

            var executionContext = new ExecutionContext();
            writer.Open(executionContext);
            try
            {
                for (int i = 0; i < CountTransactions; i++)
                {
                    using (TransactionScope scope = TransactionScopeManager.CreateScope())
                    {
                        writer.Write(GetPersons(i));
                        if (i == CountTransactions - 1) //SIMULATE FAILURE
                        {
                            throw new Exception("Bailing out ... should rollback ...");
                        }
                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
               Logger.Error(e, "An unexpected exception occured :");
            }
            writer.Close();

            var persons = new List<Person>();
            reader.Open(executionContext);
            Person person;
            while ((person = reader.Read()) != null)
            {
                persons.Add(person);
            }
            reader.Close();
            
            Assert.AreEqual((CountTransactions-1)*CountObjects,persons.Count);

            for (var i = 0; i < persons.Count; i++)
            {
                Assert.AreEqual(i%CountObjects, persons[i].Id);
                Assert.IsTrue(persons[i].Name.StartsWith("Person"+i%CountObjects));
            }
        }

        [TestMethod]
        public void FlatFileTestTest()
        {
            if (System.IO.File.Exists(_testPath))
            {
                System.IO.File.Delete(_testPath);
            }

            Assert.IsFalse(System.IO.File.Exists(_testPath));

            var writer = new FlatFileItemWriter<Person>
            {
                Resource = new FileSystemResource(_testPath),
                LineAggregator = new LineAggregator(),
                HeaderWriter = new HeaderWriter()
            };
            var reader = new FlatFileItemReader<Person>
            {
                Resource = new FileSystemResource(_testPath),
                LinesToSkip = 2,
                LineMapper = new LineMapper()
            };

            var executionContext = new ExecutionContext();
            writer.Open(executionContext);
            writer.Write(GetPersons());
            writer.Close();

            Assert.IsTrue(System.IO.File.Exists(_testPath));
            Assert.IsTrue(new FileInfo(_testPath).Length >0);

            var persons = new List<Person>();
            reader.Open(executionContext);
            Person person;
            while ((person = reader.Read()) != null)
            {
                persons.Add(person);
            }
            reader.Close();

            Assert.AreEqual(CountObjects,persons.Count);

            for (var i = 0; i < CountObjects; i++)
            {
                Assert.AreEqual(i, persons[i].Id);
                Assert.AreEqual("Person" + i, persons[i].Name);
            }
        }

        [TestMethod]
        public void FlatFileTestRestart()
        {
            System.IO.File.Copy("TestData/FlatFile/output/Test.txt", _testpath3, true);
            IResource fileResource = new FileSystemResource(new FileInfo(_testpath3));

            Assert.IsTrue(fileResource.Exists());

            var writer = new FlatFileItemWriter<Person>
            {
                Resource = new FileSystemResource(_testpath3),
                LineAggregator = new LineAggregator(),
                HeaderWriter = new HeaderWriter()
            };
            var reader = new FlatFileItemReader<Person>
            {
                Resource = new FileSystemResource(_testpath3),
                LinesToSkip = 2,
                LineMapper = new LineMapper()
            };

            var executionContext = new ExecutionContext();
            executionContext.PutLong("FlatFileItemWriter.current.count", 133L); // Last position in Test.txt
            executionContext.PutLong("FlatFileItemWriter.written", 10L); // Record already written in Test.txt 

            writer.Open(executionContext);
            writer.Write(GetPersons());
            writer.Close();

            Assert.IsTrue(System.IO.File.Exists(_testpath3));
            Assert.IsTrue(new FileInfo(_testpath3).Length > 0);

            var persons = new List<Person>();
            reader.Open(executionContext);
            Person person;
            while ((person = reader.Read()) != null)
            {
                persons.Add(person);
            }
            reader.Close();

            Assert.AreEqual(CountObjects+10, persons.Count); // The final record number must be 20+10.
        }

        private static IList<Person> GetPersons()
        {
            var persons = new List<Person>();
            for (var i = 0; i < CountObjects; i++)
            {
                persons.Add(new Person { Id = i, Name = "Person" + i });
            }
            return persons;
        }

        private static IList<Person> GetPersons(int transactionNumber)
        {
            var persons = new List<Person>();
            for (var i = 0; i < CountObjects; i++)
            {
                persons.Add(new Person { Id = i, Name = "Person" + i+"_"+transactionNumber });
            }
            return persons;
        }
    }

    [DebuggerDisplay("Id={Id}, Name={Name}")]
    class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    class LineMapper : ILineMapper<Person>
    {
        public Person MapLine(string line, int lineNumber)
        {
            var split = line.Split(new[] { ',' }, 2);
            return new Person { Id = long.Parse(split[0]), Name = split[1] };
        }
    }

    class LineAggregator : ILineAggregator<Person>
    {
        public string Aggregate(Person person)
        {
            return string.Format("{0},{1}", person.Id, person.Name);
        }
    }

    class HeaderWriter : IHeaderWriter
    {
        public void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("// Header");
            writer.Write("// Id,Name");
        }
    }
}