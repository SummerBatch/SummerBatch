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
using System.Threading;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Thread Helper.
    /// </summary>
    public static class ThreadUtils
    {
        /// <summary>
        /// Checks if the <see cref="Thread.Interrupt"/> method has been called on the current thread.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there has been a previous call to <see cref="Thread.Interrupt"/>
        /// on the current thread; <c>false</c> otherwise.
        /// </returns>
        public static bool IsCurrentThreadInterrupted()
        {
            try
            {
                Thread.Sleep(0); // get exception if interrupted.
            }
            catch (ThreadInterruptedException)
            {
                return true;
            }
            return false;
        }
    }
}