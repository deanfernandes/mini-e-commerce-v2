namespace Kafka.Contracts.Messages;

public class UserRegisteredMessage
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}