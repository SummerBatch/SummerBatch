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

namespace Summer.Batch.Extra.Process
{
    /// <summary>
    /// Interface for managing streams in readers and writters used in a process
    /// </summary>
    public interface IProcessAdapter: IDisposable
    {
        /// <summary>
        /// Registers the underlying reader/writer if it is a stream
        /// </summary>
        /// <param name="obj">a reader or writer that may be a stream</param>
        void RegisterStream(object obj);

        /// <summary>
        /// opens the stream
        /// </summary>
        void InitStream();

        /// <summary>
        /// Reset the stream
        /// </summary>
        void ResetStream();
    }
}