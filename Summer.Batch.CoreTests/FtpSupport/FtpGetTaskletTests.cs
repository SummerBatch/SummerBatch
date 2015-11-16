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
using Summer.Batch.Extra.FtpSupport;

namespace Summer.Batch.CoreTests.FtpSupport
{
    [TestClass()]
    public class FtpGetTaskletTests
    {

        [TestMethod()]
        [Ignore]
        public void DoExecuteTest()
        {
            FtpGetTasklet tasklet = new FtpGetTasklet
            {
                LocalDirectory = "C:/temp/DL",
                FileNamePattern = "*.txt",
                Host = "ftp.XXX.xxx",
                Username = "username",
                Password = "password",
                RemoteDirectory = "/"
            };

            tasklet.AfterPropertiesSet();
            Assert.IsTrue(tasklet.DoExecute());

        }

        [TestMethod()]
        public void AfterPropertiesSetTest()
        {
            FtpGetTasklet tasklet = new FtpGetTasklet
            {
                FileNamePattern = "*.txt",
                Host = "ftp.mysite.com",
                Username = "anonymous",
                Password = "changeme",
                RemoteDirectory = "/"
            };
            try
            {
                tasklet.AfterPropertiesSet();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
            tasklet.LocalDirectory = "C:/temp";
            tasklet.AfterPropertiesSet();
            Assert.IsNotNull(tasklet.LocalDirectory);
        }
    }
}
