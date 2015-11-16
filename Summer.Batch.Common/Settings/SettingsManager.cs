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
using System.Configuration;

namespace Summer.Batch.Common.Settings
{
    /// <summary>
    /// Utility class to manage application settings.
    /// 
    /// Settings and connection strings are read from the application configuration file. They can also
    /// be overriden by providing an extra configuration file using <see cref="ConfigurationFile"/>. In
    /// that case, settings and connection strings are first read in the external configuration file,
    /// then in the application configuration file.
    /// </summary>
    public class SettingsManager
    {
        private KeyValueConfigurationCollection _settings;
        private ConnectionStringSettingsCollection _connectionStrings;

        /// <summary>
        /// Sets an external configuration file as the primary source for settings and connection strings.
        /// </summary>
        public string ConfigurationFile
        {
            set
            {
                var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = value };
                var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                _settings = config.AppSettings.Settings;
                _connectionStrings = config.ConnectionStrings.ConnectionStrings;
            }
        }

        /// <summary>
        /// Retrieves a setting by its name.
        /// </summary>
        /// <param name="key">the name of the setting to retrieve</param>
        /// <returns>the value of the setting or null if it has none</returns>
        public string this[string key] { get { return Get(key); } }

        /// <summary>
        /// Retrieve setting by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            var result = _settings == null
                ? null
                : _settings[key].Value;
            return result ?? ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Retrives a connection string by its name.
        /// </summary>
        /// <param name="name">the name of the connection string to retrieve</param>
        /// <returns>the connection string, or null if it was not found</returns>
        public ConnectionStringSettings GetConnectionString(string name)
        {
            var result = _connectionStrings == null
                ? null
                : _connectionStrings[name];
            return result ?? ConfigurationManager.ConnectionStrings[name];
        }
    }
}