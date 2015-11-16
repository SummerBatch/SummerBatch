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
using Summer.Batch.Core.Converter;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Summer.Batch.CoreTests.Core.Converter
{
    [TestClass()]
    public class DefaultJobParametersConverterTests
    {
        [TestMethod()]
        public void GetJobParametersTest()
        {
            DefaultJobParametersConverter converter = new DefaultJobParametersConverter();
     
            NameValueCollection props2 = new NameValueCollection
            {
                {"+dateDebut(date)", "1970/07/31"},
                {"+everything(long)", "42"},
                {"-balance(double)", "1000.0"},
                {"+name(string)", "thierry"},
                {"-default", "default"},
                {"unmarked", "unmarked"}
            };
            JobParameters jobParameters2 = converter.GetJobParameters(props2);
            Assert.IsNotNull(jobParameters2);
            Assert.AreEqual(6,jobParameters2.GetParameters().Count);
            IDictionary<string, JobParameter> dico = jobParameters2.GetParameters();
            foreach (KeyValuePair<string, JobParameter> entry in dico)
            {
                string key = entry.Key;
                JobParameter value = entry.Value;
                Assert.IsFalse(key.StartsWith("-"));
                if ("dateDebut".Equals(key))
                {
                    Assert.AreEqual(JobParameter.ParameterType.Date,value.Type);
                    Assert.IsTrue(value.Identifying);
                }
                if ("everything".Equals(key))
                {
                    Assert.AreEqual(JobParameter.ParameterType.Long, value.Type);
                    Assert.IsTrue(value.Identifying);
                }
                if ("balance".Equals(key))
                {
                    Assert.AreEqual(JobParameter.ParameterType.Double, value.Type);
                    Assert.IsFalse(value.Identifying);
                }
                if ("name".Equals(key))
                {
                    Assert.AreEqual(JobParameter.ParameterType.String, value.Type);
                    Assert.IsTrue(value.Identifying);
                }
                if ("default".Equals(key))
                {
                    Assert.AreEqual(JobParameter.ParameterType.String, value.Type);
                    Assert.IsFalse(value.Identifying);
                }
                if ("unmarked".Equals(key))
                {
                    Assert.AreEqual(JobParameter.ParameterType.String, value.Type);
                    Assert.IsTrue(value.Identifying);
                }
            }
        }

        [TestMethod()]
        public void GetPropertiesTest()
        {
            //OrderedDictionary<string, JobParameter> myJobP = new OrderedDictionary<string, JobParameter>
            IDictionary<string, JobParameter> myJobP = new Dictionary<string, JobParameter>
            {
                {"p1", new JobParameter("param1")},
                {"p2", new JobParameter(2)},
                {"p3", new JobParameter(3.0)},
                {"p4", new JobParameter(DateTime.Parse("1970-07-31"))}
            };

            JobParameters jp = new JobParameters(myJobP);
            DefaultJobParametersConverter converter = new DefaultJobParametersConverter();
            NameValueCollection props = converter.GetProperties(jp);
            Assert.IsNotNull(props);
            Assert.AreEqual(4,props.Count);
            foreach (string key in props.Keys)
            {
                string value = props[key];
                Assert.IsNotNull(value);
                if (key.Contains("p1"))
                {
                    Assert.AreEqual("p1", key);
                }
                if (key.Contains("p2"))
                {
                    Assert.AreEqual("p2(long)", key);
                }
                if (key.Contains("p3"))
                {
                    Assert.AreEqual("p3(double)", key);
                }
                if (key.Contains("p4"))
                {
                    Assert.AreEqual("p4(date)", key);
                }
            }
        }
    }
}
