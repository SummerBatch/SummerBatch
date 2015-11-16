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
using Summer.Batch.Core;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using Summer.Batch.Common.Collections;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Core
{
    [TestClass()]
    public class JobParametersTests
    {
        private static JobParameters TearUp()
        {
            IDictionary<string, JobParameter> myJobP = new OrderedDictionary<string, JobParameter>
            {
                {"p1", new JobParameter("param1")},
                {"p2", new JobParameter(2)},
                {"p3", new JobParameter(3.0)},
                {"p4", new JobParameter(DateTime.Parse("1970-07-31"))}
            };
            JobParameters jp = new JobParameters(myJobP);
            return jp;
        }

        [TestMethod()]
        public void JobParametersTest()
        {
            JobParameters jp = new JobParameters();
            Assert.IsNotNull(jp);
            Assert.IsNotNull(jp.GetParameters());
            Assert.AreEqual(0,jp.GetParameters().Count);
        }

        [TestMethod()]
        public void JobParametersTest1()
        {
            JobParameters jp = TearUp();
            Assert.IsNotNull(jp);
            Assert.IsNotNull(jp.GetParameters());
            Assert.AreEqual(4,jp.GetParameters().Count);
        }

        [TestMethod()]
        public void GetLongTest()
        {           
            JobParameters jp = TearUp();
            long dval = jp.GetLong("p2");
            Assert.AreEqual(2,dval);
            long dval2 = jp.GetLong("unknownkey");
            Assert.IsNotNull(dval2);
            Assert.AreEqual(0,dval2);            
        }

        [TestMethod()]
        public void GetLongTest1()
        {
            JobParameters jp = TearUp();
            long dval = jp.GetLong("p2",42);
            Assert.AreEqual(2,dval);
            long dval2 = jp.GetLong("unknownkey",42);
            Assert.IsNotNull(dval2);
            Assert.AreEqual( 42, dval2);   
        }

        [TestMethod()]
        public void GetStringTest()
        {
            JobParameters jp = TearUp();
            string dval = jp.GetString("p1");
            Assert.AreEqual("param1",dval);
            string dval2 = jp.GetString("unknownkey");
            Assert.IsNull(dval2);
        }

        [TestMethod()]
        public void GetStringTest1()
        {
            JobParameters jp = TearUp();
            string dval = jp.GetString("p1","default");
            Assert.AreEqual("param1", dval);
            string dval2 = jp.GetString("unknownkey","default");
            Assert.IsNotNull(dval2);
            Assert.AreEqual("default", dval2);
        }

        [TestMethod()]
        public void GetDoubleTest()
        {
            JobParameters jp = TearUp();
            double dval = jp.GetDouble("p3");
            Assert.AreEqual( 3.0, dval);
            double dval2 = jp.GetDouble("unknownkey");
            Assert.IsNotNull(dval2);
            Assert.AreEqual( 0.0, dval2);
        }

        [TestMethod()]
        public void GetDoubleTest1()
        {
            JobParameters jp = TearUp();
            double dval = jp.GetDouble("p3", 42.0);
            Assert.AreEqual(3.0,dval);
            double dval2 = jp.GetDouble("unknownkey", 42.0);
            Assert.IsNotNull(dval2);
            Assert.AreEqual( 42.0, dval2);
        }

        [TestMethod()]
        public void GetDateTest()
        {
            JobParameters jp = TearUp();
            DateTime? dval = jp.GetDate("p4");
            Assert.IsNotNull(dval);
            Assert.AreEqual( DateTime.Parse("1970-07-31"),dval);
            DateTime? dval2 = jp.GetDate("unknownkey");
            Assert.IsNull(dval2);
        }

        [TestMethod()]
        public void GetDateTest1()
        {
            JobParameters jp = TearUp();
            DateTime defDate = DateTime.Now;
            DateTime? dval = jp.GetDate("p4",defDate);
            Assert.IsNotNull(dval);
            Assert.AreEqual( DateTime.Parse("1970-07-31"),dval);
            DateTime? dval2 = jp.GetDate("unknownkey", defDate);
            Assert.IsNotNull(dval2);
            Assert.AreEqual(defDate, dval2);
        }

        [TestMethod()]
        public void GetParametersTest()
        {
            JobParameters jp = TearUp();
            IDictionary<string,JobParameter> copy = jp.GetParameters();            
            Assert.IsNotNull(copy);
            Assert.AreEqual(4,copy.Count);
            copy.Add(new KeyValuePair<string, JobParameter>("p5", new JobParameter("newitem")));
            IDictionary<string, JobParameter> copy2 = jp.GetParameters();
            Assert.IsNotNull(copy2);
            //No modification of underlying _parameters
            Assert.AreEqual(4,copy2.Count);
            
        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            JobParameters jp = TearUp();
            Assert.IsFalse(jp.IsEmpty());
            JobParameters jp2 = new JobParameters();
            Assert.IsTrue(jp2.IsEmpty());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            IDictionary<string, JobParameter> myJobP = new Dictionary<string, JobParameter>
            {
                {"p1", new JobParameter("param1")},
                {"p2", new JobParameter(2)},
                {"p3", new JobParameter(3.0)},
                {"p4", new JobParameter(DateTime.Parse("1970-07-31"))}
            };

            JobParameters jp = new JobParameters(myJobP);

            //NOTE : ORDER DOES NOT COUNT ON EQUALS COMPARISON, ONLY <KEY,VALUES>
            //SAME BEHAVIOUR AS LinkedHashMap
            IDictionary<string, JobParameter> myJobP2 = new Dictionary<string, JobParameter>
            {
                {"p2", new JobParameter(2)},    
                {"p1", new JobParameter("param1")},                
                {"p3", new JobParameter(3.0)},
                {"p4", new JobParameter(DateTime.Parse("1970-07-31"))}
            };

            JobParameters jp2 = new JobParameters(myJobP2);
            JobParameters jp3 = new JobParameters();

            Assert.IsTrue(DictionaryUtils<string,JobParameter>.AreEqual(myJobP,myJobP2));
            Assert.IsTrue(jp.Equals(jp2));
            Assert.IsFalse(jp.Equals(jp3));
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            JobParameters jp = TearUp();
            JobParameters jpCo = TearUp();
            JobParameters jp2 = new JobParameters();
            int hc = jp.GetHashCode();
            int hcCo = jpCo.GetHashCode();
            int hc2 = jp2.GetHashCode();
            Assert.AreNotEqual(hc,hcCo);
            Assert.AreNotEqual(hc, hc2);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            JobParameters jp = TearUp();
            string jS = jp.ToString();
            Assert.IsNotNull(jS);
            Assert.AreEqual(
                  "[p1, (Parameter Type=String, Parameter Value=param1)],[p2, (Parameter Type=Long, Parameter Value=2)],[p3, (Parameter Type=Double, Parameter Value=3)],[p4, (Parameter Type=Date, Parameter Value=0)]",
                  jS);
        }
    }
}
