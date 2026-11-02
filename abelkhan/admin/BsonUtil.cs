using System.Text.Json.Nodes;
using MongoDB.Bson;

namespace admin;

/// <summary>
/// 把 MongoDB 的 BsonDocument 转成 System.Text.Json 的 JsonNode，
/// 让管理端接口返回标准 JSON（ObjectId → 字符串，DateTime → Unix 毫秒），前端直接可用。
/// </summary>
public static class BsonUtil
{
    public static JsonNode? ToJson(BsonValue value)
    {
        switch (value.BsonType)
        {
            case BsonType.ObjectId:
                return JsonValue.Create(value.AsObjectId.ToString())!;
            case BsonType.DateTime:
                return JsonValue.Create(new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeMilliseconds())!;
            case BsonType.Int32:
                return JsonValue.Create(value.AsInt32)!;
            case BsonType.Int64:
                return JsonValue.Create(value.AsInt64)!;
            case BsonType.Double:
                return JsonValue.Create(value.AsDouble)!;
            case BsonType.Decimal128:
                return JsonValue.Create(value.AsDecimal)!;
            case BsonType.String:
                return JsonValue.Create(value.AsString)!;
            case BsonType.Boolean:
                return JsonValue.Create(value.AsBoolean)!;
            case BsonType.Null:
                return null;
            case BsonType.Array:
            {
                var arr = new JsonArray();
                foreach (var item in value.AsBsonArray)
                {
                    arr.Add(ToJson(item));
                }
                return arr;
            }
            case BsonType.Document:
            {
                var obj = new JsonObject();
                foreach (var element in value.AsBsonDocument)
                {
                    obj[element.Name] = ToJson(element.Value);
                }
                return obj;
            }
            default:
                return JsonValue.Create(value.ToString())!;
        }
    }
}
