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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Serialization helper.
    /// </summary>
    public static class SerializationUtils
    {
        private static readonly string assemblyName = "Deserialization:assemblyName";
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A byte array representing the <paramref name="obj"/>.</returns>
        public static byte[] Serialize(this object obj)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// Deserializes a byte array to an object.
        /// </summary>
        /// <typeparam name="T">&nbsp;The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var serializer = new BinaryFormatter();
                serializer.Binder = new DeserializationBinder(GetBinderList());
                return (T)serializer.Deserialize(stream);
            }
        }

        private static List<string> GetBinderList()
        {
            List<string> assemblyList = new List<string>();
            IConfiguration Configuration = GetConfigurationJson();
            if (Configuration == null)
            {
                return assemblyList;
            }
            else
            {
                var list = Configuration.GetSection(assemblyName);

                if (list != null)
                {
                    foreach (var section in list.GetChildren())
                    {
                        assemblyList.Add(section.Value);
                    }
                }
            }

            return assemblyList;

        }

        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        public static IConfiguration GetConfigurationJson()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json",
                optional: true,
                reloadOnChange: true);

            return builder.Build();
        }

        public sealed class DeserializationBinder : SerializationBinder
        {

            public DeserializationBinder(List<string> list)
            {

                CustomDeserializeList = list;
            }

            public List<string> CustomDeserializeList { set; get; }

            private static readonly List<string> SummerBatchCore = new List<string>() { "Summer.Batch.Common", "Summer.Batch.Core", "Summer.Batch.Data", "Summer.Batch.Extra", "Summer.Batch.Infrastructure", "mscorlib", "System", "Microsoft" };
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                Assembly currentAssembly = Assembly.Load(assemblyName);

                //Get List of Class Name
                string Name = currentAssembly.GetName().Name;
                if ((SummerBatchCore.Contains(Name) ||  SummerBatchCore.Any(name => Regex.IsMatch(Name, WildCardToRegular(name + "*")))) || 
                    (CustomDeserializeList.Count != 0 && CustomDeserializeList.Any(name => Regex.IsMatch(Name, WildCardToRegular(name)))))
                {
                    //The following line of code returns the type.
                    typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, Name));
                }
                else
                {
                    throw new SerializationException("Failed to deserialize. Please create appsettings.json and add assembly name into assembly section of Deserialization.");
                }

                return typeToDeserialize;
            }
        }
    }
}
