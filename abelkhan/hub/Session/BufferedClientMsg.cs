namespace hub;

/// <summary>
/// 迁移切割后缓冲的一条客户端请求。新 hub replay 时用原始 connId/msgId，
/// 回包路径（clients[userId] → gate → conn）不受 hub 变化影响，所以客户端无感。
/// </summary>
public sealed record BufferedClientMsg(string ConnId, string MsgId, string Method, Google.Protobuf.ByteString Content);
