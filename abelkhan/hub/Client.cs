namespace hub;

public class Client(string userId, string gateName, string connId)
{
    public string UserId => userId;
    public string GateName => gateName;
    public string ConnId => connId;
}