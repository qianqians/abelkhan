namespace hub;

public class Client(string entityId, string userId, string connId)
{
    public string EntityId => entityId;
    public string UserId => userId;
    public string ConnId => connId;
}