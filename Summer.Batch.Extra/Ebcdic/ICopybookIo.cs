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
namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    ///  Interface for classes using copybooks for reading or writing EBCDIC files.
    /// </summary>
    public interface ICopybookIo
    {
        /// <summary>
        /// Changes the current copybook
        /// </summary>
        /// <param name="copybook">the simple name (without extension) of the copybook</param>
        void ChangeCopyBook(string copybook);
    }
}