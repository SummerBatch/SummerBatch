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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

//   This file has been modified.
//   Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;

namespace Summer.Batch.Core
{
    /// <summary>
    ///  Value object used to carry information about the status of a
    /// job or step execution.
    /// ExitStatus is immutable and therefore thread-safe.
    /// </summary>

    [Serializable]
    public sealed class ExitStatus : IComparable<ExitStatus>
    {
        ///<summary>
        ///Convenient constant value representing unknown state - assumed not
        ///continuable.
        ///</summary>
        public static readonly ExitStatus Unknown = new ExitStatus("UNKNOWN");

        ///<summary>
        ///Convenient constant value representing continuable state where processing
        ///is still taking place, so no further action is required. Used for
        ///asynchronous execution scenarios where the processing is happening in
        ///another thread or process and the caller is not required to wait for the
        ///result.
        ///</summary>
        public static readonly ExitStatus Executing = new ExitStatus("EXECUTING");

        ///<summary>
        ///Convenient constant value representing finished processing.
        ///</summary>
        public static readonly ExitStatus Completed = new ExitStatus("COMPLETED");

        ///<summary>
        ///Convenient constant value representing job that did no processing (e.g.
        ///because it was already complete).
        ///</summary>
        public static readonly ExitStatus Noop = new ExitStatus("NOOP");

        ///<summary>
        ///Convenient constant value representing finished processing with an error.
        ///</summary>
        public static readonly ExitStatus Failed = new ExitStatus("FAILED");

        ///<summary>
        ///Convenient constant value representing finished processing with
        ///interrupted status.
        ///</summary>
        public static readonly ExitStatus Stopped = new ExitStatus("STOPPED");

        private readonly string _exitCode;

        /// <summary>
        /// Exit code/
        /// </summary>
        public string ExitCode
        {
            get { return _exitCode; }
        }

        private readonly string _exitDescription;

        /// <summary>
        /// Exit description.
        /// </summary>
        public string ExitDescription
        {
            get { return _exitDescription; }
        }

        /// <summary>
        /// Custom constructor using a code and a description.
        /// </summary>
        /// <param name="exitCode"></param>
        /// <param name="exitDescription"></param>
        public ExitStatus(string exitCode, string exitDescription = "")
        {
            _exitCode = exitCode;
            _exitDescription = exitDescription ?? "";
        }


        ///<summary>
        ///Create a new ExistStatus with a logical combination of the exit
        ///code, and a concatenation of the descriptions. If either value has a
        ///higher severity then its exit code will be used in the result. In the
        ///case of equal severity, the exit code is replaced if the new value is
        ///alphabetically greater.
        ///
        ///Severity is defined by the exit code:
        ///<ul>
        ///<li>Codes beginning with EXECUTING have severity 1</li>
        ///<li>Codes beginning with COMPLETED have severity 2</li>
        ///<li>Codes beginning with NOOP have severity 3</li>
        ///<li>Codes beginning with STOPPED have severity 4</li>
        ///<li>Codes beginning with FAILED have severity 5</li>
        ///<li>Codes beginning with UNKNOWN have severity 6</li>
        ///</ul>
        ///Others have severity 7, so custom exit codes always win.
        ///If the input is null just return this.
        ///<param name="status">an ExitStatus to combine with this one.</param> 
        ///<returns>a new ExitStatus combining the current value and the
        /// argument provided.</returns> 
        ///</summary>
        public ExitStatus And(ExitStatus status)
        {
            if (status == null)
            {
                return this;
            }
            ExitStatus result = AddExitDescription(status._exitDescription);
            if (CompareTo(status) < 0)
            {
                result = result.ReplaceExitCode(status._exitCode);
            }
            return result;
        }

        ///<summary>
        ///<param name="status"> an ExitStatus to compare </param>
        ///<returns> greater than zero, 0, less than zero according to the severity and exit code</returns>
        /// see IComparable
        ///</summary>
        public int CompareTo(ExitStatus status)
        {
            if (Severity(status) > Severity(this))
            {
                return -1;
            }
            if (Severity(status) < Severity(this))
            {
                return 1;
            }
            return string.Compare(_exitCode, status._exitCode, StringComparison.Ordinal);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private int Severity(ExitStatus status)
        {
            int result = 7;//default result
            if (status._exitCode.StartsWith(Executing._exitCode))
            {
                result= 1;
            }
            if (status._exitCode.StartsWith(Completed._exitCode))
            {
                result= 2;
            }
            if (status._exitCode.StartsWith(Noop._exitCode))
            {
                result= 3;
            }
            if (status._exitCode.StartsWith(Stopped._exitCode))
            {
                result= 4;
            }
            if (status._exitCode.StartsWith(Failed._exitCode))
            {
                result= 5;
            }
            if (status._exitCode.StartsWith(Unknown._exitCode))
            {
                result= 6;
            }
            return result;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(exitCode={0};exitDescription={1})", _exitCode, _exitDescription);
        }

        /// <summary>
        /// Compare the fields one by one.
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Compatible with the Equals implementation.
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        ///<summary>
        ///Add an exit code to an existing ExistStatus. If there is already a
        ///code present, it will be replaced.	
        ///<param name="code">code</param>
        ///<returns> a new ExistStatus with the same properties but a new exit </returns>
        ///code.
        ///</summary>
        public ExitStatus ReplaceExitCode(string code)
        {
            return new ExitStatus(code, _exitDescription);
        }


        /// <summary>
        /// Check if this status represents a running process.
        /// <returns>true if the exit code is "EXECUTING" or "UNKNOWN"</returns>
        /// </summary>
        public bool IsRunning()
        {
            return "EXECUTING".Equals(_exitCode) || "UNKNOWN".Equals(_exitCode);
        }

        /// <summary>
        ///Add an exit description to an existing ExistStatus. If there is
        ///already a description present the two will be concatenated with a
        ///semicolon.
        ///<param name="description">the description to add</param> 
        ///<returns> a new ExitStatus with the same properties but a new exit </returns>
        ///description
        /// </summary>
        public ExitStatus AddExitDescription(string description)
        {
            StringBuilder buffer = new StringBuilder();
            bool changed = !string.IsNullOrWhiteSpace(description) && !_exitDescription.Equals(description);
            if (!string.IsNullOrWhiteSpace(description))
            {
                buffer.Append(_exitDescription);
                if (changed)
                {
                    buffer.Append("; ");
                }
            }
            if (changed)
            {
                buffer.Append(description);
            }
            return new ExitStatus(_exitCode, buffer.ToString());
        }

        ///<summary>
        ///Extract the stack trace from the throwable provided and append it to
        ///the exist description.	
        ///<param name="exception">the given exception</param> 
        ///<returns> a new ExitStatus with the stack trace appended</returns>
        ///</summary>
        public ExitStatus AddExitDescription(Exception exception)
        {
            return AddExitDescription(exception.StackTrace);
        }

        ///<summary>
        ///<param name="status"> the exit code to be evaluated</param>
        ///<returns> true if the value matches a known exit code </returns>
        ///</summary>
        public static bool IsNonDefaultExitStatus(ExitStatus status)
        {
            return status == null || status._exitCode == null ||
                    status.ExitCode.Equals(Completed.ExitCode) ||
                    status.ExitCode.Equals(Executing.ExitCode) ||
                    status.ExitCode.Equals(Failed.ExitCode) ||
                    status.ExitCode.Equals(Noop.ExitCode) ||
                    status.ExitCode.Equals(Stopped.ExitCode) ||
                    status.ExitCode.Equals(Unknown.ExitCode);
        }
    }
}
