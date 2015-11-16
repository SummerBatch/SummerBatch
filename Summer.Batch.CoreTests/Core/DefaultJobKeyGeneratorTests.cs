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
using System;
using System.Collections.Generic;

namespace Summer.Batch.CoreTests.Core
{
    [TestClass()]
    public class DefaultJobKeyGeneratorTests
    {
        [TestMethod()]
        public void GenerateKeyTest()
        {
            IDictionary<string, JobParameter> myJobP = new Dictionary<string, JobParameter>
            {
                {"p1", new JobParameter("param1")},
                {"p2", new JobParameter(2)},
                {"p3", new JobParameter(3.0)},
                {"p4", new JobParameter(DateTime.Parse("1970-07-31"))}
            };
            JobParameters jp = new JobParameters(myJobP);
            DefaultJobKeyGenerator dkg = new DefaultJobKeyGenerator();
            string key = dkg.GenerateKey(jp);
            Assert.IsNotNull(key);
            Assert.AreEqual("1d07b3d07378e9fd9afa323fd8c69cc0", key);
        }
    }
}
