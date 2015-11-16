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
using System.Collections.Generic;
using NLog;
using Summer.Batch.Extra.Ebcdic;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    public class CompositeItem : Item, IEbcdicBusinessObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string DistinguishedValue { get { return "2"; } }

        public string Type
        {
            get
            {

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Start of Method CompositeItem.getType");
                }
                var objRet = PrGetType();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("End of Method CompositeItem.getType");
                    Logger.Debug("returns  : " + objRet);
                }
                return objRet;
            }
        }

        private string PrGetType()
        {
            return "2";
        }


        public int NbItems
        {
            get
            {

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Start of Method CompositeItem.getNbItems");
                }
                var objRet = PrGetNbItems();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("End of Method CompositeItem.getNbItems");
                    Logger.Debug("returns  : " + objRet);
                }
                return objRet;
            }
        }

        private int PrGetNbItems()
        {
            return _items.Count;
        }

        private ICollection<SingleItem> _items = new HashSet<SingleItem>();

        public ICollection<SingleItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        //public void RemoveFromItems(SingleItem entity)
        //{
        //    if (_items != null && _items.Contains(entity))
        //    {
        //        _items.Remove(entity);
        //        entity.Container = null;
        //    }
        //}

        //public void AddToItems(SingleItem entity)
        //{
        //    if (_items != null && !_items.Contains(entity))
        //    {
        //        _items.Add(entity);
        //        entity.Container = this;
        //    }
        //}

        //public void RemoveAllFromItems(ICollection<SingleItem> entities)
        //{
        //    foreach (SingleItem entity in entities)
        //    {
        //        if (_items != null && _items.Contains(entity))
        //        {
        //            RemoveFromItems(entity);
        //        }
        //    }
        //}

        //public void RemoveAllFromItems()
        //{
        //    ICollection<SingleItem> entities = _items;
        //    if (entities == null)
        //    {
        //        return;
        //    }
        //    foreach (SingleItem entity in entities)
        //    {
        //        entity.Container = null;
        //    }
        //    entities.Clear();
        //}
    }
}