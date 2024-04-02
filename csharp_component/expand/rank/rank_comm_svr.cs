using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
    public class rank_item
    {
        public Int64 guid;
        public Int64 score;
        public Int32 rank;
        public byte[] item;
        public static MsgPack.MessagePackObjectDictionary rank_item_to_protcol(rank_item _struct){
        if (_struct == null) {
            return null;
        }

            var _protocol = new MsgPack.MessagePackObjectDictionary();
            _protocol.Add("guid", _struct.guid);
            _protocol.Add("score", _struct.score);
            _protocol.Add("rank", _struct.rank);
            _protocol.Add("item", _struct.item);
            return _protocol;
        }
        public static rank_item protcol_to_rank_item(MsgPack.MessagePackObjectDictionary _protocol){
        if (_protocol == null) {
            return null;
        }

            var _struct8e307fd3_8e06_3971_adaf_10e42d498ea0 = new rank_item();
            foreach (var i in _protocol){
                if (((MsgPack.MessagePackObject)i.Key).AsString() == "guid"){
                    _struct8e307fd3_8e06_3971_adaf_10e42d498ea0.guid = ((MsgPack.MessagePackObject)i.Value).AsInt64();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "score"){
                    _struct8e307fd3_8e06_3971_adaf_10e42d498ea0.score = ((MsgPack.MessagePackObject)i.Value).AsInt64();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "rank"){
                    _struct8e307fd3_8e06_3971_adaf_10e42d498ea0.rank = ((MsgPack.MessagePackObject)i.Value).AsInt32();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "item"){
                    _struct8e307fd3_8e06_3971_adaf_10e42d498ea0.item = ((MsgPack.MessagePackObject)i.Value).AsBinary();
                }
            }
            return _struct8e307fd3_8e06_3971_adaf_10e42d498ea0;
        }
    }

/*this caller code is codegen by abelkhan codegen for c#*/
/*this module code is codegen by abelkhan codegen for c#*/

}
