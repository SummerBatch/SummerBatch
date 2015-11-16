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

namespace Summer.Batch.Core.Step.Tasklet
{
    /// <summary>
    /// An extension to the <see cref="ITasklet"/>interface to allow users to
    /// add logic for stopping a tasklet.  It is up to each implementation
    /// as to how the stop will behave.  The only guarantee provided by the
    /// framework is that a call to JobOperator.Stop(long) will
    /// attempt to call the stop method on any currently running
    /// IStoppableTasklet.  The call to IStoppableTasklet.Stop() will
    /// be from a thread other than the thread executing Tasklet.Execute()
    /// so the appropriate thread safety and visibility controls should be
    /// put in place.
    /// </summary>
    public interface IStoppableTasklet : ITasklet
    {
        /// <summary>
        /// Used to signal that the job this Tasklet is executing within has been requested to stop.
        /// </summary>
        void Stop();
    }
}
