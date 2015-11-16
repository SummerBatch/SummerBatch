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
using System.Data;

namespace Summer.Batch.Data.Parameter
{
    /// <summary>
    /// Class providing and extension to facilitate adding parameters to command.
    /// </summary>
    public static class ParameterExtension
    {
        /// <summary>
        /// Adds a new parameter to a command
        /// </summary>
        /// <param name="command">the command to add a parameter to</param>
        /// <param name="name">the name of the parameter to add</param>
        /// <param name="value">(optional) the value of the parameter</param>
        public static void AddParameter(this IDbCommand command, string name, object value = null)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            if (value != null)
            {
                parameter.Value = value;
            }
            command.Parameters.Add(parameter);
        }
    }
}
