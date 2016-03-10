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
    /// Maps the exit code of a PowerShell to <see cref="ExitStatus"/> value
    /// returned by a PowerShell. Designed for use with the
    /// <see cref="PowerShellTasklet"/>.
    /// 
    /// \since 1.1.0
    /// </summary>
    public interface IPowerShellExitCodeMapper
    {
        /// <summary>
        /// </summary>
        /// <param name="exitCode">exit code returned by the PowerShell</param>
        /// <returns>ExitStatus appropriate for the systemExitCode parameter value</returns>
        ExitStatus GetExitStatus(int exitCode);
    }
}

