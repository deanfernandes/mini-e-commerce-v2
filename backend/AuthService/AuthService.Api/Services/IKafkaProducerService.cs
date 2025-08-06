using Kafka.Contracts.Messages;

namespace AuthService.Api.Services
{
    public interface IKafkaProducerService
    {
        Task ProduceUserRegisteredAsync(UserRegisteredMessage @event, CancellationToken cancellationToken = default);
    }
}