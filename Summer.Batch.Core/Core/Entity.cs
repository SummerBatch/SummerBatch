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

//   This file has been modified.
//   Original copyright notice :

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

using System;

namespace Summer.Batch.Core
{
    /// <summary>
    /// Batch Domain Entity class. Any class that should be uniquely identifiable
    /// from another should subclass from Entity. More information on this pattern
    /// and the difference between Entities and Value Objects can be found in Domain
    /// Driven Design by Eric Evans.
    /// </summary>
    [Serializable]
    public class Entity
    {
        /// <summary>
        /// Id.
        /// </summary>
        public long? Id {get; set;}

        /// <summary>
        /// Version.
        /// </summary>
        public int? Version {get; set;}

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Entity() {}

        /// <summary>
        /// Custom constructor with id.
        /// </summary>
        /// <param name="id"></param>
        public Entity(long? id) {
            Id = id;
        }

        /// <summary>
        /// Increments the version number.
        /// </summary>
        public void IncrementVersion() {
            if (Version == null) {
                Version = 0;
            } else {
                Version = Version + 1;
            }
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>    
        public  override string ToString() {
            return string.Format("{0}: id={1}, version={2}", GetType().Name, Id, Version);
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Object other) {
            if (other == this) {
                return true;
            }
            if (!(other is Entity)) {
                return false;
            }
            var entity = (Entity) other;
            if (Id == null || entity.Id == null) {
                return false;
            }
            return Id.Equals(entity.Id);
        }

        /// <summary>
        /// Use ID if it exists to establish hash code, otherwise fall back to a call
        /// og GetHashCode on the result of the ToString method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            if (Id == null) {
                return ToString().GetHashCode();
            }
            return 39 + 87 * Id.GetHashCode();
        }
    }
}
