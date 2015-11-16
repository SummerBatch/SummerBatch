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
using NLog;
using Summer.Batch.Common.Util;
using System;

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// Object helper.
    /// </summary>
    public static class ObjectUtils
    {
        // Logger declaration.
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Check if the given object is null.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>true if object is null. Otherwise false.</returns> 
        public static bool IsNull(object obj)
        {
            return obj==null;
        }

        /// <summary>
        /// Check the given objects are equal.
        /// </summary>
        /// <param name="obj1">object</param>  
        /// <param name="obj2">object</param>
        /// <returns>true if obj1 is the same as obj2. Otherwise false.</returns> 
        public static bool AreEqual(object obj1, object obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// ToString method.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>a string representation of the object.</returns> 
        public static string ToString(object obj)
        {
            return obj.ToString();
        }
    
        /// <summary>
        /// Copy property values from the origin bean to the destination bean for all cases where the property names are the same.
        /// </summary>
        /// <param name="dest">object</param>
        /// <param name="orig">object</param>
        public static void CopyProperties(object dest, object orig)
        {
            try {
                CopyUtils.CopyProperties(dest, orig);
            } catch (ArgumentNullException e) {
                Logger.Error(e,"An error occured while executing method: CopyProperties ");
            }
        }
    
        /// <summary>
        /// Return the object of a given type from a set of object.
        /// </summary>
        /// <param name="resultset">object</param>  
        /// <param name="type">string</param>  
        /// <returns>the object of the given type.</returns> 
        public static object GetObjectByType(object[] resultset, String type)
        {
            object result = null;
            if (resultset!=null)
            {
                foreach (object res in resultset)
                {
                    if (res != null)
                    {
                        var typeObj = res.GetType().Name;
                        var simpleRefType = (StringUtils.IndexOf(type,'.') > 0);
                        if (simpleRefType) 
                        {
                            typeObj = StringUtils.Substring(typeObj,StringUtils.LastIndexOf(typeObj,'.')+1,typeObj.Length);
                        }
                        if (type.Equals(typeObj, StringComparison.InvariantCultureIgnoreCase))
                        {
                            result = res;
                            break;
                        }
                    }
                }
            }
            return result;			
        }
    }
}
