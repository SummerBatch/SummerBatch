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
using Summer.Batch.Common.Util;
using Summer.Batch.CoreTests.Util.Test;
using System.Collections.Generic;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public class CopyUtilsTests
    {
        [TestMethod]
        public void CopyPropertiesTest() {

            var pi1 = new PhoneItem() { FaxNumber = "456598784532", PhoneNumber = "124565789854" };
            var pi2 = new PhoneItem() { FaxNumber = "789874654312", PhoneNumber = "684763251325" };
            var phoneItems = new List<PhoneItem>() { pi1, pi2 };
            var address = new Address() { Id = 1, City = "Rennes", Street = "rue de la paix" };

            var objOrigin = new Employee { Id = 0, Name = "Bernard", Password = "123456789", Address = address, PhoneItems = phoneItems };
            objOrigin.ModifieAge(5);

            var objDestination = new Employee();

            CopyUtils.CopyProperties(objDestination, objOrigin);

            Assert.IsTrue(objDestination.Id.Equals(objOrigin.Id));
            Assert.IsTrue(objDestination.Name.Equals(objOrigin.Name));
            Assert.IsTrue(objDestination.Password.Equals(objOrigin.Password));
            Assert.IsTrue(objDestination.GetAge().Equals(0));
            Assert.IsTrue(objDestination.Address.Equals(objOrigin.Address));
            Assert.IsTrue(objDestination.PhoneItems.Equals(objOrigin.PhoneItems));
        }

        [TestMethod]
        public void CopyPropertiesTest1() {

            var pi1 = new PhoneItem() { FaxNumber = "456598784532", PhoneNumber = "124565789854" };
            var pi2 = new PhoneItem() { FaxNumber = "789874654312", PhoneNumber = "684763251325" };
            var phoneItems = new List<PhoneItem>() { pi1, pi2 };
            var address = new Address() { Id = 1, City = "Rennes", Street = "rue de la paix" };

            var objOrigin = new Employee { Id = 0, Name = "Bernard", Password = "123456789", Address = address, PhoneItems = phoneItems };
            var objDestination = new Customer();

            CopyUtils.CopyProperties(objDestination, objOrigin);

            Assert.IsTrue(objDestination.Id.Equals(objOrigin.Id));
            Assert.IsNull(objDestination.GetName());
            Assert.IsNull(objDestination.Address);
            Assert.IsNull(objDestination.Emails);
        }

        [TestMethod]
        public void CopyPropertiesTest2()
        {
            var address = new CompleteAddress() { Id = 0, City = "Rennes", Street = "rue de la paix", State=33, Country="France" };
            var email1 = "test.@gmail.com";
            var email2 = "test2@hotmail.com"; 
            var emails = new List<string>() { email1, email2};

            var objOrigin = new Customer { Id = 0, Address = address, Emails = emails };
            var objDestination = new Employee();

            CopyUtils.CopyProperties(objDestination, objOrigin);

            Assert.IsTrue(objDestination.Id.Equals(objOrigin.Id));
            Assert.IsNull(objDestination.Name);
            Assert.IsTrue(objDestination.Address.Equals(objOrigin.Address));
        }
    }
}
