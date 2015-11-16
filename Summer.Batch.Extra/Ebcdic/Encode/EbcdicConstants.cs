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
namespace Summer.Batch.Extra.Ebcdic.Encode
{
    /// <summary>
    /// Class holding constants used for encoding and decoding EBCDIC
    /// </summary>
    public static class EbcdicConstants
    {
        /// <summary>
        /// ZonedPositives constant.
        /// </summary>
        public static readonly char[] ZonedPositives = "{ABCDEFGHI".ToCharArray();

        /// <summary>
        /// ZonedNegatives constant.
        /// </summary>
        public static readonly char[] ZonedNegatives = "}JKLMNOPQR".ToCharArray();
        
        /// <summary>
        /// MfMinus constant.
        /// </summary>
        public static readonly char[] MfMinus = "pqrstuvwxy".ToCharArray();

        /// <summary>
        /// CaMinus constant.
        /// </summary>
        public static readonly char[] CaMinus = " !\"#$%&'()".ToCharArray();

        /// <summary>
        /// HexTable constant.
        /// </summary>
        public static readonly char[] HexTable = "0123456789ABCDEF".ToCharArray();
    }
}