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

// This file has been modified.
// Original copyright notice :

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
namespace Summer.Batch.Infrastructure.Support
{
    /// <summary>
    /// PatternMatcher
    /// </summary>
    public static class PatternMatcher
    {
        ///<summary>
        ///Lifted from AntPathMatcher in Spring Core. Tests whether or not a string
        ///matches against a pattern. The pattern may contain two special
        ///characters:
        ///'*' means zero or more characters
        ///'?' means one and only one character
        ///
        ///<param name="pattern"> pattern to match against. Must not be null.</param> 
        ///<param name="str">string which must be matched against the pattern. Must not be null.</param> 
        ///<returns>true if the string matches against the pattern, or false otherwise.</returns> 
        ///</summary>
        public static bool Match(string pattern, string str)
        {
            char[] patArr = pattern.ToCharArray();
            char[] strArr = str.ToCharArray();
            int patIdxStart = 0;
            int patIdxEnd = patArr.Length - 1;
            int strIdxStart = 0;
            int strIdxEnd = strArr.Length - 1;
            char ch;

            bool containsStar = pattern.Contains("*");

            if (!containsStar)
            {
                // No '*'s, so we make a shortcut
                if (patIdxEnd != strIdxEnd)
                {
                    return false; // Pattern and string do not have the same size
                }
                for (int i = 0; i <= patIdxEnd; i++)
                {
                    ch = patArr[i];
                    if (ch != '?' && ch != strArr[i])
                    {
                        return false; // Character mismatch                     
                    }
                }
                return true; // String matches against pattern
            }

            if (patIdxEnd == 0)
            {
                return true; // Pattern contains only '*', which matches anything
            }

            // Process characters before first star
            while ((ch = patArr[patIdxStart]) != '*' && strIdxStart <= strIdxEnd)
            {
                if (ch != '?' && ch != strArr[strIdxStart])
                {
                    return false; // Character mismatch                 
                }
                patIdxStart++;
                strIdxStart++;
            }
            if (strIdxStart > strIdxEnd)
            {
                // All characters in the string are used. Check if only '*'s are
                // left in the pattern. If so, we succeeded. Otherwise failure.
                for (int i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (patArr[i] != '*')
                    {
                        return false;
                    }
                }
                return true;
            }

            // Process characters after last star
            while ((ch = patArr[patIdxEnd]) != '*' && strIdxStart <= strIdxEnd)
            {
                if (ch != '?' && ch != strArr[strIdxEnd])
                {
                    return false; // Character mismatch                 
                }
                patIdxEnd--;
                strIdxEnd--;
            }
            if (strIdxStart > strIdxEnd)
            {
                // All characters in the string are used. Check if only '*'s are
                // left in the pattern. If so, we succeeded. Otherwise failure.
                for (int i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (patArr[i] != '*')
                    {
                        return false;
                    }
                }
                return true;
            }

            // process pattern between stars. padIdxStart and patIdxEnd point
            // always to a '*'.
            while (patIdxStart != patIdxEnd && strIdxStart <= strIdxEnd)
            {
                int patIdxTmp = -1;
                for (int i = patIdxStart + 1; i <= patIdxEnd; i++)
                {
                    if (patArr[i] == '*')
                    {
                        patIdxTmp = i;
                        break;
                    }
                }
                if (patIdxTmp == patIdxStart + 1)
                {
                    // Two stars next to each other, skip the first one.
                    patIdxStart++;
                    continue;
                }
                // Find the pattern between padIdxStart & padIdxTmp in str between
                // strIdxStart & strIdxEnd
                int patLength = (patIdxTmp - patIdxStart - 1);
                int strLength = (strIdxEnd - strIdxStart + 1);
                int foundIdx = -1;

                int i2 = 0;
                strLoop:
                while (i2 <= strLength - patLength)
                {
                    for (int j = 0; j < patLength; j++)
                    {
                        ch = patArr[patIdxStart + j + 1];
                        if (ch != '?' && ch != strArr[strIdxStart + i2 + j])
                        {
                            // SEE https://msdn.microsoft.com/en-us/library/aa664757%28v=vs.71%29.aspx
                            // "When multiple while, do, for, or foreach statements are nested within each other, 
                            // a continue statement applies only to the innermost statement. 
                            // To transfer control across multiple nesting levels, a goto statement (Section 8.9.3) must be used."
                            i2++;
                            goto strLoop; //buying some technical debt   }:P                            
                        }
                    }

                    foundIdx = strIdxStart + i2;
                    break;
                }

                if (foundIdx == -1)
                {
                    return false;
                }

                patIdxStart = patIdxTmp;
                strIdxStart = foundIdx + patLength;
            }

            // All characters in the string are used. Check if only '*'s are left
            // in the pattern. If so, we succeeded. Otherwise failure.
            for (int i = patIdxStart; i <= patIdxEnd; i++)
            {
                if (patArr[i] != '*')
                {
                    return false;
                }
            }

            return true;
        }

    }
}