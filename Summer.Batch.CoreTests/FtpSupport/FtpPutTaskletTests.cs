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
    public class FtpPutTaskletTests
    {
        [TestMethod()]
        [Ignore]
        public void DoExecuteTest()
        {
            FtpPutTasklet tasklet = new FtpPutTasklet
            {
                FileName = "C:/temp/MyDummyTasklet_out_UP.txt",
                Host = "ftp.XXX.xxx",
                Username = "username",
                Password = "password" 
            };            
            tasklet.AfterPropertiesSet();
            Assert.IsTrue(tasklet.DoExecute());
        }

        [TestMethod()]
        public void AfterPropertiesSetTest()
        {
            FtpPutTasklet tasklet = new FtpPutTasklet
            {
                FileName = "C:/temp/MyDummyTasklet_out_UP.txt",
                Username = "anonymous",
                Password = "changeme"
            };

            try
            {
                tasklet.AfterPropertiesSet();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);                    
            }
            tasklet.Host = "ftp.mysite.com";
            tasklet.AfterPropertiesSet();
            Assert.IsNotNull(tasklet.Host);
        }
    }
}
