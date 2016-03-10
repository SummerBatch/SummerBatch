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
using System;
using System.Reflection;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Copy Helper.
    /// </summary>
    public static class CopyUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Copy properties from an origin object to a destination object. 
        /// A property is copied if :
        /// - the property names are the same in both destination and origin object.
        /// A property is skipped if :
        /// - a property exists in the origin object but do not exist in the destination object.
        /// - a property is not readable in the origin object.
        /// - a property is not writable in the destination object.
        /// - the destination property type is not assignable from the origin property type.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="origin"></param>
        /// <exception cref="ArgumentNullException">&nbsp;</exception>
        public static void CopyProperties(object destination, object origin)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (origin == null)
            {
                throw new ArgumentNullException("origin");
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("CopyUtils.CopyProperties({0},{1})",destination,origin);
            }
            foreach (PropertyInfo piOrigin in origin.GetType().GetProperties())
            {
                var piDestination = destination.GetType().GetProperty(piOrigin.Name);
                //do all needed checks
                if (CheckDestinationNullity(piDestination, piOrigin))
                {
                    continue;
                }
                if (CheckOriginReadability(piOrigin))
                {
                    continue;
                }
                if (CheckDestinationWritability(piDestination, piOrigin))
                {
                    continue;
                }
                if (CheckCompatibility(piDestination, piOrigin))
                {
                    continue;
                }
                //performs copy if all tests have succeeded
                piDestination.SetValue(destination, piOrigin.GetValue(origin));
            }
        }

        /// <summary>
        /// Checks compatibility between origin and destination.
        /// </summary>
        /// <param name="piDestination"></param>
        /// <param name="piOrigin"></param>
        /// <returns></returns>
        private static bool CheckCompatibility(PropertyInfo piDestination, PropertyInfo piOrigin)
        {
            if (!piDestination.PropertyType.IsAssignableFrom(piOrigin.PropertyType))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Property {0}  of type {1} is not assignable from type {2}.",
                        piDestination.Name, piOrigin.PropertyType.Name, piOrigin.PropertyType.Name);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks that destination can be written to.
        /// </summary>
        /// <param name="piDestination"></param>
        /// <param name="piOrigin"></param>
        /// <returns></returns>
        private static bool CheckDestinationWritability(PropertyInfo piDestination, PropertyInfo piOrigin)
        {
            if (!piDestination.CanWrite)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Property {0} is not writable in the destination object.", piOrigin.Name);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks that origin can be read.
        /// </summary>
        /// <param name="piOrigin"></param>
        /// <returns></returns>
        private static bool CheckOriginReadability(PropertyInfo piOrigin)
        {
            if (!piOrigin.CanRead)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Property {0} is not readable in the origin object.", piOrigin.Name);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks that destination is not null.
        /// </summary>
        /// <param name="piDestination"></param>
        /// <param name="piOrigin"></param>
        /// <returns></returns>
        private static bool CheckDestinationNullity(PropertyInfo piDestination, PropertyInfo piOrigin)
        {
            if (piDestination == null)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Property {0} does not exist in destination object.", piOrigin.Name);
                }
                return true;
            }
            return false;
        }
    }
}
