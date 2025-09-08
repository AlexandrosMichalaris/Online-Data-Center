namespace Data_Center.Configuration.Options;

public class RedisOptions
{
    public bool AbortOnConnectFail { get; set; } = false;
    
    public int ConnectRetry { get; set; } = 3;
    
    public int ConnectTimeout { get; set; } = 5000;
    
    public int SyncTimeout { get; set; } = 5000;
    
    public int KeepAlive { get; set; } = 60;
    
    public bool AllowAdmin { get; set; } = false;
    
    public int DefaultDatabase { get; set; } = 0;
    
    public string? Password { get; set; }
    
    public bool Ssl { get; set; } = false;
    
    public string? SslHost { get; set; }
}