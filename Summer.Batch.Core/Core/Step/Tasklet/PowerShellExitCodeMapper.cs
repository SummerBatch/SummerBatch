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


namespace Summer.Batch.Core.Step.Tasklet
{
    /// <summary>
    /// Simple ISystemProcessExitCodeMapper implementation that performs following mapping:
    /// 0 	-&gt; ExitStatus.Completed
    /// else	-&gt; ExitStatus.Failed
    /// 
    /// \since 1.1.0
    /// </summary>
    public class PowerShellExitCodeMapper : IPowerShellExitCodeMapper
    {
        /// <summary>
        /// @see IPowerShellExitCodeMapper#GetExitStatus.
        /// </summary>
        /// <param name="exitCode"></param>
        /// <returns></returns>
        public ExitStatus GetExitStatus(int exitCode)
        {
            if (exitCode == 0)
            {
                return ExitStatus.Completed;
            }
            else
            {
                return ExitStatus.Failed;
            }
        }
    }
}
