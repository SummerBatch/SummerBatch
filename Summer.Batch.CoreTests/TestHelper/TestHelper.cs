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
using System.IO;
using Summer.Batch.Common.Extensions;

namespace Summer.Batch.CoreTests.TestHelper
{
    public static class TestHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns></returns>
        /// <exception cref="IOException">&nbsp;</exception>
        public static bool ContentEquals(Stream input1, Stream input2)
        {
            if (!(input1 is BufferedStream))
            {
                input1 = new BufferedStream(input1);
            }
            if (!(input2 is BufferedStream))
            {
                input2 = new BufferedStream(input2);
            }
            var bytes1 = new byte[1];
            var bytes2 = new byte[1];
            while (input1.Read(bytes1) == 1)
            {
                if (input2.Read(bytes2) == 0 || bytes1[0] != bytes2[0])
                {
                    return false;
                }
            }

            return input2.Read(bytes2) == 0;
        }
    }
}