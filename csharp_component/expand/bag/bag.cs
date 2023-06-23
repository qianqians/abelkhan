using Abelkhan;
using avatar;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using static MongoDB.Driver.WriteConcern;

namespace bag
{
    public class Bag : avatar.IHostingData
    {
        private int capacity;
        private List<item_def> items;

        public static string Type()
        {
            return "bag";
        }

        public static IHostingData Create()
        {
            return new Bag()
            {
                capacity = BagModdule.capacity,
                items = new List<item_def>()
            };
        }

        public static IHostingData Load(BsonDocument data)
        {
            var bag = new Bag()
            {
                capacity = data["capacity"].AsInt32,
                items = new List<item_def>()
            };
            foreach(var _item in data.GetValue("items").AsBsonArray)
            {
                var item = new item_def();
                item.uuid = _item["uuid"].AsInt64;
                item.desc_id = _item["desc_id"].AsInt64;
                item.amount = _item["amount"].AsInt32;
                bag.items.Add(item);
            }

            return bag;
        }

        public BsonDocument Store()
        {
            var itemList = new BsonArray();
            foreach (var item in items)
            {
                itemList.Add(new BsonDocument
                {
                    { "uuid", item.uuid },
                    { "desc_id", item.desc_id },
                    { "amount", item.amount }
                });
            }
            var doc = new BsonDocument
            {
                { "capacity", capacity },
                { "items", itemList }
            };
            return doc;
        }

        // use item uuid
        public event Func<long, bool> onUseItem;
        public bool use_item(long uuid)
        {
            if (onUseItem == null)
            {
                return false;
            }
            return onUseItem.Invoke(uuid);
        }

        public Abelkhan.bag_def get_bag()
        {
            var bag = new Abelkhan.bag_def();
            bag.capacity = capacity;
            bag.items = items;
            return bag;
        }

        public void add_capacity(int count)
        {
            capacity += count;
        }

        public bool add_item(ref item_def _item)
        {
            var amount = 0;
            foreach(var it in items)
            {
                if (it.desc_id == _item.desc_id)
                {
                    amount = (it.amount + _item.amount) - it.limit_amount;
                    if (amount <= 0)
                    {
                        it.amount += _item.amount;
                        _item.amount = 0;
                        return true;
                    }
                }
            }

            if (amount > 0)
            {
                _item.amount = amount;
                if (capacity > items.Count)
                {
                    items.Add(_item);
                    return true;
                }
            }

            return false;
        }

        public bool sub_item(long uuid)
        {
            foreach (var it in items)
            {
                if (it.uuid == uuid)
                {
                    it.amount--;
                    if (it.amount <= 0)
                    {
                        items.Remove(it);
                    }
                    return true;
                }
            }

            return false;
        }

        public bool del_item(long uuid)
        {
            foreach (var it in items)
            {
                if (it.uuid == uuid)
                {
                    items.Remove(it);
                    return true;
                }
            }

            return false;
        }
    }

    public static class BagModdule
    {
        public static int capacity = 30;
        private static AvatarMgr avatarMgr;
        private static bag_service_module bag_Service_Module = new();

        public static void init(int _capacity, AvatarMgr mgr)
        {
            capacity = _capacity;
            avatarMgr = mgr;

            avatarMgr.add_hosting<Bag>();

            bag_Service_Module.on_get_bag += Bag_Service_Module_on_get_bag;
            bag_Service_Module.on_use_item += Bag_Service_Module_on_use_item;
        }

        private static void Bag_Service_Module_on_use_item(long uuid)
        {
            var rsp = bag_Service_Module.rsp as bag_service_use_item_rsp;

            var avatar = avatarMgr.get_current_avatar();
            var bag = avatar.get_real_hosting_data<Bag>();
            if (bag.Data.use_item(uuid))
            {
                rsp.rsp();
            }
            else
            {
                rsp.err();
            }
        }

        private static void Bag_Service_Module_on_get_bag()
        {
            var rsp = bag_Service_Module.rsp as bag_service_get_bag_rsp;

            var avatar = avatarMgr.get_current_avatar();
            var bag = avatar.get_real_hosting_data<Bag>();
            rsp.rsp(bag.Data.get_bag());
        }
    }
}
