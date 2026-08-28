using core;

namespace hub;

public class Client(string userId, string gateName, string connId)
{
    private string _userId = userId;
    private string _gateName = gateName;
    private string _connId = connId;

    public string UserId
    {
        get
        {
            LastEventTime = TimerService.Tick;
            return _userId;
        }
        set => _userId = value;
    }

    public string GateName
    {
        get
        {
            LastEventTime = TimerService.Tick;
            return _gateName;
        }
        set => _gateName = value;
    }
    public string ConnId {
        get
        {
            LastEventTime = TimerService.Tick;
            return _connId;
        }    
        set => _connId = value;
    }

    public long LastEventTime { private set; get; }
}