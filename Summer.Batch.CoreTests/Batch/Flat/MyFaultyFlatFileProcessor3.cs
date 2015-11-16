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
using System;
using NLog;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.CoreTests.Batch.Flat
{
    public class MyFaultyFlatFileProcessor3 : IItemProcessor<Person, Person>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Person Process(Person entity)
        {
            if (entity.Firstname.Equals("Wesley") && entity.Name.Equals("Snipes"))
            {
                Person newEntity = null;
                string newFirstName = newEntity.Firstname;
                Logger.Info("newFirstName =" + newFirstName);
            }
            entity.BirthYear = entity.Birth.Year;
            return entity;
        }
    }
}