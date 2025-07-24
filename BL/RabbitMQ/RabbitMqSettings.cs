namespace BL.RabbitMQ;

public class RabbitMqSettings
{
    public string? HostName { get; set; } = "localhost";
    public int Port { get; set; }
    public string? UserName { get; set; } = "guest";
    public string? Password { get; set; } = "password";
}