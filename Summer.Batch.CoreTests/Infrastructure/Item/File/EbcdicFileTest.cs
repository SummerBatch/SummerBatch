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
using System.IO;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.CoreTests.Ebcdic.Test;
using Summer.Batch.Extra.Ebcdic;
using Summer.Batch.Extra.Ebcdic.Encode;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Transaction;

namespace Summer.Batch.CoreTests.Infrastructure.Item.File
{
    [TestClass]
    public class EbcdicFileTest
    {
        /// <summary>
        /// bigger count means a bigger file and a bigger exec time
        /// metrics : 
        /// 10000 => 225 Ko file on my pc (Test1)
        /// 10000 => 1456 Ko file on my pc (Test2)
        /// 1000000 => 22 Mo file on my pc (Test1 - took ~ 15 sec. to finish - DEBUG LOGGER turned off)
        /// 1000000 => 142 Mo file on my pc (Test2 - took ~ 43 sec. to finish - DEBUG LOGGER turned off)
        /// </summary>
        private const int CountObject = 100;
        private const int CountTransactions = 3;

        [TestMethod]
        public void EbcdicFileTestTest()
        {
            // local resources
            IResource fileResource = new FileSystemResource(new FileInfo("C:/temp/outputs/PersonWritten.txt"));
            var executionContext = new ExecutionContext();
            FileInfo fileInfo = new FileInfo("Ebcdic/Resources/copybooks/Person.fileformat");

            IResource copybookResource = new FileSystemResource(fileInfo);

            //1. WRITE
            EbcdicWriterMapper writerMapper = new EbcdicWriterMapper();
            writerMapper.AfterPropertiesSet();

            var writer = new EbcdicFileWriter<Ebcdic.Test.Person>
            {
                AppendAllowed = false,
                WriteRdw = false,
                Name = "PersonWriter",
                Resource = fileResource,
                EbcdicWriterMapper = writerMapper,
                DefaultValue = EbcdicEncoder.DefaultValue.LowValue,
                Copybooks = new List<IResource> {new FileSystemResource(fileInfo)}
            };


            writer.AfterPropertiesSet();
            writer.Open(executionContext);
            writer.Write(GetPersons());
            writer.Close();

            //2.READ WHAT WAS WRITTEN
            var reader = new EbcdicFileReader<Ebcdic.Test.Person>
            {
                EbcdicReaderMapper = new PersonMapper(),
                Rdw = false,
                Resource = fileResource,
                Name = "PersonReader",
                SaveState = false,
                Copybook = copybookResource
            };
            reader.AfterPropertiesSet();

            var persons = new List<Ebcdic.Test.Person>();
            reader.Open(executionContext);
            Ebcdic.Test.Person person;
            while ((person = reader.Read()) != null)
            {
                persons.Add(person);
            }
            reader.Close();

            Assert.AreEqual(CountObject,persons.Count );
            foreach (Ebcdic.Test.Person p in persons)
            {
                Assert.AreEqual(p.Id, p.Value);
                Assert.IsTrue(p.Name.StartsWith("Name_" + p.Id));
            }
        }

        [TestMethod]
        public void EbcdicFileTestTest2()
        {
            // local resources
            IResource fileResource = new FileSystemResource(new FileInfo("C:/temp/outputs/CustomerWritten.txt"));
            var executionContext = new ExecutionContext();
            FileInfo fileInfo = new FileInfo("Ebcdic/Resources/copybooks/Customer.fileformat");

            IResource copybookResource = new FileSystemResource(fileInfo);

            //1. WRITE
            EbcdicWriterMapper writerMapper = new EbcdicWriterMapper();
            writerMapper.AfterPropertiesSet();

            var writer = new EbcdicFileWriter<Customer>
            {
                AppendAllowed = false,
                WriteRdw = false,
                Name = "CustomerWriter",
                Resource = fileResource,
                EbcdicWriterMapper = writerMapper,
                DefaultValue = EbcdicEncoder.DefaultValue.LowValue,
                Copybooks = new List<IResource> {new FileSystemResource(fileInfo)}
            };


            writer.AfterPropertiesSet();
            writer.Open(executionContext);
            writer.Write(GetCustomers());
            writer.Close();

            //2.READ WHAT WAS WRITTEN
            var reader = new EbcdicFileReader<Customer>
            {
                EbcdicReaderMapper = new CustomerEbcdicMapper(),
                Rdw = false,
                Resource = fileResource,
                Name = "CustomerReader",
                SaveState = false,
                Copybook = copybookResource
            };
            reader.AfterPropertiesSet();

            var customers = new List<Customer>();
            reader.Open(executionContext);
            Customer customer;
            while ((customer = reader.Read()) != null)
            {
                customers.Add(customer);
            }
            reader.Close();

            Assert.AreEqual(CountObject,customers.Count);

        }

        [TestMethod]
        public void EbcdicFileTestTransactionalWrite()
        {
            // local resources
            IResource fileResource = new FileSystemResource(new FileInfo("C:/temp/outputs/PersonWritten.txt"));
            var executionContext = new ExecutionContext();
            FileInfo fileInfo = new FileInfo("Ebcdic/Resources/copybooks/Person.fileformat");

            IResource copybookResource = new FileSystemResource(fileInfo);

            //1. WRITE
            EbcdicWriterMapper writerMapper = new EbcdicWriterMapper();
            writerMapper.AfterPropertiesSet();

            var writer = new EbcdicFileWriter<Ebcdic.Test.Person>
            {
                AppendAllowed = false,
                WriteRdw = false,
                Name = "PersonWriter",
                Resource = fileResource,
                EbcdicWriterMapper = writerMapper,
                DefaultValue = EbcdicEncoder.DefaultValue.LowValue,
                Copybooks = new List<IResource> {new FileSystemResource(fileInfo)}
            };

            writer.AfterPropertiesSet();
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
            catch (Exception)
            {
               // DISCARDED (JUST TO AVOID UNIT TEST FAILURE ...)
            }
            writer.Close();

            Assert.IsTrue(System.IO.File.Exists("C:/temp/outputs/PersonWritten.txt"));
            Assert.IsTrue(new FileInfo("C:/temp/outputs/PersonWritten.txt").Length > 0);

            //2.READ WHAT WAS WRITTEN
            var reader = new EbcdicFileReader<Ebcdic.Test.Person>
            {
                EbcdicReaderMapper = new PersonMapper(),
                Rdw = false,
                Resource = fileResource,
                Name = "PersonReader",
                SaveState = false,
                Copybook = copybookResource
            };
            reader.AfterPropertiesSet();

            var persons = new List<Ebcdic.Test.Person>();
            reader.Open(executionContext);
            Ebcdic.Test.Person person;
            while ((person = reader.Read()) != null)
            {
                persons.Add(person);
            }
            reader.Close();

            Assert.AreEqual(CountObject*(CountTransactions-1), persons.Count);
            foreach (Ebcdic.Test.Person p in persons)
            {
                Assert.AreEqual(p.Id, p.Value);
                Assert.IsTrue(p.Name.StartsWith("Name_" + p.Id));
            }
        }

        [TestMethod]
        public void EbcdicFileTestRestart() {
            // local resources
            System.IO.File.Copy("Ebcdic/Resources/outputs/simple.txt", "C:/temp/outputs/SimpleWritten.txt", true);
            IResource fileResource = new FileSystemResource(new FileInfo("C:/temp/outputs/SimpleWritten.txt"));
            var executionContext = new ExecutionContext();
            executionContext.PutLong("EbcdicFileWriter.current.count",21L);
            executionContext.PutLong("EbcdicFileWriter.written", 1L);
            FileInfo fileInfo = new FileInfo("Ebcdic/Resources/copybooks/simple.fileformat");
            
            IResource copybookResource = new FileSystemResource(fileInfo);

            //1. WRITE
            EbcdicWriterMapper writerMapper = new EbcdicWriterMapper();
            writerMapper.AfterPropertiesSet();

            var writer = new EbcdicFileWriter<Simple>
            {
                AppendAllowed = false,
                WriteRdw = false,
                Resource = fileResource,
                EbcdicWriterMapper = writerMapper,
                DefaultValue = EbcdicEncoder.DefaultValue.LowValue,
                DeleteIfExist = true
            };


            var mySimple = new Simple() { LongValue = 12, FloatValue = 12.5f, Date = DateTime.Now, BooleanValue = true };
            List<Simple> simpleList = new List<Simple> { mySimple };

            writer.Copybooks = new List<IResource> { new FileSystemResource(fileInfo) };

            writer.AfterPropertiesSet();
            writer.Open(executionContext);
            writer.Write(simpleList);
            writer.Close();

            //2.READ WHAT WAS WRITTEN
            var reader = new EbcdicFileReader<Simple>
            {
                EbcdicReaderMapper = new SimpleEbcdicMapper(),
                Rdw = false,
                Resource = fileResource,
                Name = "PersonReader",
                SaveState = false,
                Copybook = copybookResource
            };
            reader.AfterPropertiesSet();

            var simples = new List<Simple>();
            reader.Open(executionContext);
            Simple simple;
            while ((simple = reader.Read()) != null)
            {
                simples.Add(simple);
            }
            reader.Close();

            /*foreach (Ebcdic.Test.Person p in simple)
            {
                Assert.AreEqual(p.Id, p.Value);
                Assert.IsTrue(p.Name.StartsWith("Name_" + p.Id));
            }*/
        }

        private List<Customer> GetCustomers()
        {
            List<Customer> result = new List<Customer>();
            for (int i = 0; i < CountObject; i++)
            {
                result.Add(new Customer{
                    Name = "Name_" + i,
                    Id = i,
                    Addresses = new List<CustomerAddress>
                    {
                        new CustomerAddress()
                        {
                            City = "Bordeaux",
                            Street = "Rue de la victoire",
                            PhoneItem = new PhoneItem()
                            {
                                FaxNumber = (i%10) + "1234567",
                                PhoneNumber = "8765432" + (i%10)
                            }
                        }
                    },
                    Emails = new List<string> { "email" + i + "test.com", "altemail" + i + "test.com" }
                });
            }
            return result;
        }

        private List<Ebcdic.Test.Person> GetPersons()
        {
            List<Ebcdic.Test.Person> result = new List<Ebcdic.Test.Person>();
            for (int i = 0; i < CountObject; i++)
            {
                result.Add(new Ebcdic.Test.Person()
                {
                    Name = "Name_" + i,
                    Value = i,
                    Id = i
                });
            }
            return result;
        }

        private List<Ebcdic.Test.Person> GetPersons(int transactionNumber)
        {
            List<Ebcdic.Test.Person> result = new List<Ebcdic.Test.Person>();
            for (int i = 0; i < CountObject; i++)
            {
                result.Add(new Ebcdic.Test.Person()
                {
                    Name = "Name_"+i+"_"+ transactionNumber,
                    Value = i,
                    Id = i
                });
            }
            return result;
        }

    }
}