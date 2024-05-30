﻿using Abelkhan;
using MongoDB.Bson;
using Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace Rank
{
    public class Rank
    {
        internal string name;
        internal int capacity;
        internal int total_capacity;
        internal long base_score;
        internal SortedList<long, rank_item> rankList = new();
        internal SortedDictionary<long, long> guidRank = new ();

        public Rank(int _capacity)
        {
            capacity = _capacity;
            total_capacity = _capacity + 200;
        }

        public BsonDocument ToBsonDocument()
        {
            var doc = new BsonDocument
            {
                { "name", name },
                { "capacity", capacity }
            };

            var docRankList = new BsonDocument();
            foreach (var it in rankList)
            {
                var docItem = new BsonDocument
                {
                    { "guid", it.Value.guid },
                    { "score", it.Value.score },
                    { "rank", it.Value.rank },
                    { "item", it.Value.item }
                };
                docRankList.Add(it.Key.ToString(), docItem);
            }
            doc.Add("rankList", docRankList);

            var docGuidRank = new BsonDocument();
            foreach(var it in guidRank)
            {
                docGuidRank.Add(it.Key.ToString(), it.Value);
            }
            doc.Add("guidRank", docGuidRank);

            return doc;
        }

        public int UpdateRankItem(rank_item item)
        {
            if (guidRank.TryGetValue(item.guid, out var oldScore))
            {
                rankList.Remove(oldScore);
            }

            var score = item.score << 32 | (int.MaxValue - Timerservice.Tick / 1000);
            if (score < base_score)
            {
                return -1;
            }

            rankList.Add(score, item);
            item.rank = rankList.IndexOfKey(score) + 1;
            guidRank[item.guid] = score;

            if (rankList.Count > total_capacity) {
                var remove = new List<long>();
                var removeGuid = new List<long>();
                for (var i = total_capacity; i < rankList.Count; ++i)
                {
                    remove.Add(rankList.GetKeyAtIndex(i));
                    removeGuid.Add(rankList.GetValueAtIndex(i).guid);
                }
                foreach (var key in remove)
                {
                    rankList.Remove(key);
                }
                foreach (var guid in removeGuid)
                {
                    guidRank.Remove(guid);
                }

                base_score = rankList.GetKeyAtIndex(total_capacity - 1);
            }

            return item.rank;
        }

        public void ResetRank()
        {
            rankList.Clear();
            guidRank.Clear();

            base_score = 0;
        }

        public rank_item GetRankGuid(long guid)
        {
            var rank = new rank_item
            {
                guid = guid
            };

            if (!guidRank.TryGetValue(guid, out var score))
            {
                rank.rank = -1;
                rank.score = 0;
            }
            else
            {
                if (rankList.TryGetValue(score, out var r))
                {
                    rank.rank = rankList.IndexOfKey(score) + 1;
                    rank.score = score;
                    rank.item = r.item;
                }
            }

            return rank;
        }

        public List<rank_item> GetRankRange(int start, int end)
        {
            var rank = new List<rank_item>();

            var r = start;
            var c = rankList.Count < capacity ? rankList.Count : capacity;
            end = end < c ? end : c;
            for (var i = start - 1; i < end; i++)
            {
                var item = rankList.GetValueAtIndex(i);
                item.rank = r++;
                rank.Add(item);
            }

            return rank;
        }
    }

    public static class RankModule
    {
        private static string dbName;
        private static string dbCollection;

        private static Dictionary<string, Rank> rankDict = new();

        private static rank_cli_service_module rank_Cli_Service_Module = new();
        private static rank_svr_service_module rank_svr_Service_Module = new();

        public static Task Init(string _dbName, string _dbCollection, List<string> rankNames)
        {
            var task = new TaskCompletionSource();

            rank_Cli_Service_Module.on_get_rank_guid += Rank_Cli_Service_Module_on_get_rank_guid;
            rank_Cli_Service_Module.on_get_rank_range += Rank_Cli_Service_Module_on_get_rank_range;

            rank_svr_Service_Module.on_get_rank_guid += Rank_svr_Service_Module_on_get_rank_guid;
            rank_svr_Service_Module.on_update_rank_item += Rank_svr_Service_Module_on_update_rank_item;

            dbName = _dbName;
            dbCollection = _dbCollection;

            var query = new DBQueryHelper();
            query._in("name", new BsonArray(rankNames));
            Hub.Hub.get_random_dbproxyproxy().getCollection(dbName, dbCollection).getObjectInfo(query.query(), (array) =>
            {
                foreach(var rankDoc in array)
                {
                    var rank = new Rank(rankDoc["capacity"].AsInt32);
                    rank.name = rankDoc["name"].AsString;

                    foreach (var it in rankDoc["rankList"].AsBsonDocument)
                    {
                        var item = new rank_item();
                        item.guid = it.Value["guid"].AsInt64;
                        item.score = it.Value["score"].AsInt64;
                        item.rank = it.Value["rank"].AsInt32;
                        item.item = it.Value["item"].AsBsonBinaryData.Bytes;
                        rank.rankList.Add(long.Parse(it.Name), item);
                    }

                    foreach(var it in rankDoc["guidRank"].AsBsonDocument)
                    {
                        rank.guidRank.Add(long.Parse(it.Name), it.Value.AsInt32);
                    }

                    rankDict.Add(rank.name, rank);
                }

                foreach(var rName in rankNames)
                {
                    if (!rankDict.ContainsKey(rName))
                    {
                        var rank = new Rank(100);
                        rank.name = rName;
                        rankDict.Add(rank.name, rank);
                    }
                }

            }, () =>
            {
                task.SetResult();
            });

            Hub.Hub._timer.addticktime(60 * 60 * 1000, tick_save_rank);

            return task.Task;
        }

        public static Rank get_rank(string name)
        {
            rankDict.TryGetValue(name, out var rank);
            return rank;
        }

        public static void save_rank()
        {
            try
            {
                foreach (var item in rankDict.Values)
                {
                    var query = new DBQueryHelper();
                    query.condition("name", item.name);
                    var doc = item.ToBsonDocument();
                    var update = new UpdateDataHelper();
                    update.set(doc);
                    Hub.Hub.get_random_dbproxyproxy().getCollection(dbName, dbCollection).updataPersistedObject(query.query(), update.data(), false, (result) =>
                    {
                        if (result != Hub.DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                        {
                            Log.Log.err($"tick_save_rank item.name faild:{result}");
                        }
                    });
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err($"tick_save_rank:{ex}");
            }
        }

        private static void tick_save_rank(long tick)
        {
            save_rank();
            Hub.Hub._timer.addticktime(60 * 60 * 1000, tick_save_rank);
        }

        public static void reset_rank(string rankName)
        {
            if (rankDict.TryGetValue(rankName, out var rankIns))
            {
                rankIns.ResetRank();
            }
        }

        private static void Rank_svr_Service_Module_on_update_rank_item(string rankNmae, rank_item item)
        {
            var rsp = rank_svr_Service_Module.rsp as rank_svr_service_update_rank_item_rsp;

            try
            {
                if (rankDict.TryGetValue(rankNmae, out var rankIns))
                {
                    var rank = rankIns.UpdateRankItem(item);
                    rsp.rsp(rank);
                }
                else
                {
                    rsp.err();
                }
            }
            catch(System.Exception ex)
            {
                Log.Log.err("on_update_rank_item:{0}", ex);
            }
        }

        private static void Rank_svr_Service_Module_on_get_rank_guid(string rankNmae, long guid)
        {
            var rsp = rank_svr_Service_Module.rsp as rank_svr_service_get_rank_guid_rsp;

            try
            {
                if (rankDict.TryGetValue(rankNmae, out var rankIns))
                {
                    var rank = rankIns.GetRankGuid(guid);
                    rsp.rsp(rank.rank);
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("on_get_rank_guid:{0}", ex);
            }
        }

        private static void Rank_Cli_Service_Module_on_get_rank_range(string rankNmae, int start, int end)
        {
            var rsp = rank_Cli_Service_Module.rsp as rank_cli_service_get_rank_range_rsp;

            try
            {
                if (rankDict.TryGetValue(rankNmae, out var rankIns))
                {
                    var rank = rankIns.GetRankRange(start, end);
                    rsp.rsp(rank);
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("on_get_rank_range:{0}", ex);
            }
        }

        private static void Rank_Cli_Service_Module_on_get_rank_guid(string rankNmae, long guid)
        {
            var rsp = rank_Cli_Service_Module.rsp as rank_cli_service_get_rank_guid_rsp;

            try
            {
                if (rankDict.TryGetValue(rankNmae, out var rankIns))
                {
                    var rank = rankIns.GetRankGuid(guid);
                    rsp.rsp(rank);
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("on_get_rank_guid:{0}", ex);
            }
        }
    }
}
