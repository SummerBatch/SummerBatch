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
using Summer.Batch.Extra.Ebcdic;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    public class SingleItem : Item,IEbcdicBusinessObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CompositeItem Container { get; set; }

        public double Price { get; set; }

        private string PrGetType()
        {
            return "1";
        }

        private string PrGetEmpty()
        {
            return " ";
        }

        public string Type
        {
            get
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Start of Method SingleItem.GetType");
                }
                var objRet = PrGetType();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("End of Method SingleItem.GetType");
                    Logger.Debug("returns :" + objRet);
                }
                return objRet;
            }
        }

        public string Empty
        {
            get
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Start of Method SingleItem.GetEmpty");
                }
                var objRet = PrGetEmpty();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("End of Method SingleItem.GetEmpty");
                    Logger.Debug("returns :" + objRet);
                }
                return objRet;
            }
        }
        public string DistinguishedValue { get { return "1"; }}
    }
}