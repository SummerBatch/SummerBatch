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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;
using Summer.Batch.Extra.EmailSupport;

namespace Summer.Batch.CoreTests.EmailSupport
{
    [TestClass()]
    public class EmailTaskletTests
    {
        [TestMethod()]
        // NOTE : Test is commited as ignored to prevent spam on every build action ... ;)
        // Remove Ignore on purpose (to actually test the mail sending feature)
        [Ignore]
        public void DoExecuteTest()
        {
            EmailTasklet tasklet = new EmailTasklet();
            SetupTasklet(tasklet, 0);
            tasklet.AfterPropertiesSet();
            Assert.IsTrue(tasklet.DoExecute());
        }

        [TestMethod()]
        // NOTE : Test is commited as ignored to prevent spam on every build action ... ;)
        // Remove Ignore on purpose (to actually test the mail sending feature)
        [Ignore]
        public void DoExecuteTest2()
        {
            EmailTasklet tasklet = new EmailTasklet();
            SetupTasklet(tasklet, 30);
            tasklet.AfterPropertiesSet();
            Assert.IsTrue(tasklet.DoExecute());
        }

        [TestMethod()]
        public void AfterPropertiesSetTest()
        {
            EmailTasklet tasklet = new EmailTasklet();
            try
            {
                tasklet.AfterPropertiesSet();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
            SetupTasklet(tasklet, 0);

            tasklet.AfterPropertiesSet();
            Assert.IsNotNull(tasklet);
        }

        private static void SetupTasklet(EmailTasklet tasklet, int lineLength)
        {
            tasklet.Username = "summerbatch.sender@XXX.xxx";
            tasklet.Password = "******";
            tasklet.Encoding = Encoding.UTF8;
            tasklet.From = "summerbatch.sender@XXX.xxx";
            tasklet.To = new[] { "t.masson@bluage.com" };
            tasklet.Cc = new[] { "r.delamare@bluage.com" };
            tasklet.Bcc = new[] { "s.lesaffre@bluage.com" };
            tasklet.Host = "srvXXXX.xxx";
            tasklet.Subject = "EmailTasklet test; please do not respond, this is an automatically sent email.";
            tasklet.Body = new FileSystemResource(@"TestData\Email\sampleMailBody.txt");
            tasklet.LineLength = lineLength;
        }
    }
}
