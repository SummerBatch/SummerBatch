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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using NLog;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.EmailSupport
{
    /// <summary>
    /// Tasklet for sending emails. The host, from, subject and body fields are
    /// mandatory. The body is read from a resource. At least one recipient should be
    /// specified (in to, cc, or bcc).
    /// </summary>
    public class EmailTasklet : ITasklet, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default Smpt port is 25.
        /// </summary>
        private const int DefaultSmtpPort = 25;

        #region Attributes
        /// <summary>
        /// Smtp Username.
        /// </summary>
        public string Username { private get; set; }
        /// <summary>
        /// Smtp Password for above stmp username.
        /// </summary>
        public string Password { private get; set; }

        /// <summary>
        /// Smtp port (defaults to 25).
        /// </summary>
        private int _port = DefaultSmtpPort;

        /// <summary>
        /// Port property.
        /// </summary>
        public int Port { set { _port = value; } }

        /// <summary>
        /// Smtp Host.
        /// </summary>
        public string Host { private get; set; }

        /// <summary>
        /// mail sender address.
        /// </summary>
        public string From { private get; set; }

        /// <summary>
        /// list of recipients
        /// </summary>
        private string[] _to = { };

        /// <summary>
        /// List of recipients property.
        /// </summary>
        public string[] To
        {
            set
            {
                _to = new string[value.Length];
                Array.Copy(value, _to, value.Length);
            }
        }

        /// <summary>
        /// list of cc recipients.
        /// </summary>
        private string[] _cc = { };

        /// <summary>
        /// list of cc recipients property.
        /// </summary>
        public string[] Cc
        {
            set
            {
                _cc = new string[value.Length];
                Array.Copy(value, _cc, value.Length);
            }
        }

        /// <summary>
        /// list of Bcc recipients.
        /// </summary>
        private string[] _bcc = { };
        
        /// <summary>
        /// list of Bcc recipients property.
        /// </summary>
        public string[] Bcc
        {
            set
            {
                _bcc = new string[value.Length]; 
                Array.Copy(value, _bcc, value.Length);
            }
        }

        /// <summary>
        /// Mail subject
        /// </summary>
        public string Subject { private get; set; }

        /// <summary>
        /// A resource containing the body of the e-mail.
        /// </summary>
        public IResource Body { private get; set; }

        /// <summary>
        /// The maximum line length of the e-mail.
        /// </summary>
        public int LineLength { private get; set; }

        /// <summary>
        /// Encoding of the e-mail.
        /// </summary>
        private Encoding _encoding = Encoding.Default;
        
        /// <summary>
        /// Encoding of the e-mail property.
        /// </summary>
        public Encoding Encoding
        {
            set { _encoding = value; }
        }

        /// <summary>
        /// Encoding of <see cref="Body"/>.
        /// </summary>
        private Encoding _inputEncoding = Encoding.Default;

        /// <summary>
        /// Input encoding of the e-mail property.
        /// </summary>
        public Encoding InputEncoding
        {
            set { _inputEncoding = value; }
        }

        #endregion

        /// <summary>
        /// Send the mail message given all needed information.
        /// @see ITasklet#Execute
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            DoExecute();//Delegated
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Inner execute.
        /// </summary>
        /// <returns></returns>
        public bool DoExecute()
        {            
            //1) Setup message
            var message = CreateMailMessage();
            //2) Setup client
            var client = SetupSmtpClient();
            //3) send message using client
            client.Send(message);
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Mail from {0} has been sent to [{1}], cc=[{2}], bcc=[{3}] ",
                    From, string.Join(",", _to), string.Join(",", _cc), string.Join(",", _bcc));
            }
            Logger.Info("Mail from {0} has been sent.", From);
            return true;
        }

        /// <summary>
        /// Setup smtp client
        /// </summary>
        /// <returns></returns>
        private SmtpClient SetupSmtpClient()
        {
            SmtpClient client = new SmtpClient
            {
                Host = Host,
                Port = _port,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            if (Username != null && Password != null)
            {
                client.UseDefaultCredentials = false;                
                client.Credentials = new NetworkCredential(Username, Password);
            }
            return client;
        }

        /// <summary>
        /// Create mail message
        /// </summary>
        /// <returns></returns>
        private MailMessage CreateMailMessage()
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(From),
                Subject = Subject,
                Body = GetText()
            };
            if (_to.Any())
            {
                foreach (var dest in _to)
                {
                    message.To.Add(new MailAddress(dest));
                }
            }
            if (_cc.Any())
            {
                foreach (var dest in _cc)
                {
                    message.CC.Add(new MailAddress(dest));
                }
            }

            if (_bcc.Any())
            {
                foreach (var dest in _bcc)
                {
                    message.Bcc.Add(new MailAddress(dest));
                }
            }
            return message;
        }


        /// <summary>
        /// Read mail body from given file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        private string GetText()
        {
            //Read file into string
            string text;
            using (var reader = new StreamReader(Body.GetInputStream(), _inputEncoding))
            {
                text = LineLength <= 0 ? reader.ReadToEnd() : GetLines(reader);
            }
            return text;
        }

        /// <summary>
        /// Read mail body, line by line of the specified LineLength size.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string GetLines(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (reader.Peek() >= 0)
            {
                var buffer = new char[LineLength];
                reader.ReadBlock(buffer, 0, LineLength);
                sb.AppendLine(new string(buffer));
            }
            return sb.ToString();
        }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Host, "EmailTasklet : The Host must be specified");
            Assert.NotNull(From, "EmailTasklet : The From address must be specified");
            Assert.State(_to.Any() || _cc.Any() || _bcc.Any(), "EmailTasklet : at least one recipient must be specified");
            Assert.NotNull(Subject, "EmailTasklet : the subject must be specified");
            Assert.NotNull(Body, "EmailTasklet : the body resource must be specified");
            Assert.State(Body.Exists(), "EmailTasklet : the body resource must exist");
            Assert.IsTrue(LineLength >= 0, "LineLength : if specified, the line LineLength must be equals or superior to 0");
        }
    }
}