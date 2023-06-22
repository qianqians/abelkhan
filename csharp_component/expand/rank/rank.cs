﻿using Abelkhan;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace Rank
{
    public class Rank
    {
        internal string name;
        internal int capacity;
        internal SortedList<long, rank_item> rankList = new();
        internal SortedDictionary<long, long> guidRank = new ();

        public Rank(int _capacity)
        {
            capacity = _capacity;
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

            var score = item.score << 32 | (int.MaxValue - item.guid);
            rankList.Add(score, item);
            item.rank = rankList.IndexOfKey(score) + 1;
            guidRank[item.guid] = score;

            if (rankList.Count > (capacity + 200)) {
                var remove = new List<long>();
                for (var i = capacity; i < rankList.Count; ++i)
                {
                    remove.Add(rankList.GetKeyAtIndex(i));
                }
                foreach (var key in remove)
                {
                    rankList.Remove(key);
                }
            }

            return item.rank;
        }

        public int GetRankGuid(long guid)
        {
            if (!guidRank.TryGetValue(guid, out var score))
            {
                return -1;
            }

            for (var r = 0; r < rankList.Count; r++)
            {
                var item = rankList[r];
                if (item.score == score)
                {
                    return r + 1;
                }
            }

            return -1;
        }

        public List<rank_item> GetRankRange(int start, int end)
        {
            var rank = new List<rank_item>();

            var r = start;
            for (var i = rankList.Count - start; i >= rankList.Count - end && i >= 0; --i)
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

        public static Task Init(string _dbName, string _dbCollection)
        {
            var task = new TaskCompletionSource();

            rank_Cli_Service_Module.on_get_rank_guid += Rank_Cli_Service_Module_on_get_rank_guid;
            rank_Cli_Service_Module.on_get_rank_range += Rank_Cli_Service_Module_on_get_rank_range;

            rank_svr_Service_Module.on_get_rank_guid += Rank_svr_Service_Module_on_get_rank_guid;
            rank_svr_Service_Module.on_update_rank_item += Rank_svr_Service_Module_on_update_rank_item;

            dbName = _dbName;
            dbCollection = _dbCollection;

            var query = new DBQueryHelper();
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

            }, () =>
            {
                task.SetResult();
            });

            Hub.Hub._timer.addticktime(60 * 60 * 1000, tick_save_rank);

            return task.Task;
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

        private static void Rank_svr_Service_Module_on_update_rank_item(string rankNmae, rank_item item)
        {
            var rsp = rank_svr_Service_Module.rsp as rank_svr_service_update_rank_item_rsp;

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

        private static void Rank_svr_Service_Module_on_get_rank_guid(string rankNmae, long guid)
        {
            var rsp = rank_svr_Service_Module.rsp as rank_svr_service_get_rank_guid_rsp;

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

        private static void Rank_Cli_Service_Module_on_get_rank_range(string rankNmae, int start, int end)
        {
            var rsp = rank_Cli_Service_Module.rsp as rank_cli_service_get_rank_range_rsp;

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

        private static void Rank_Cli_Service_Module_on_get_rank_guid(string rankNmae, long guid)
        {
            var rsp = rank_Cli_Service_Module.rsp as rank_cli_service_get_rank_guid_rsp;

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
    }
}
