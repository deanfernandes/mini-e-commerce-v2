using Kafka.Contracts.Messages;

namespace AuthService.Api.Services
{
    public interface IKafkaProducerService
    {
        public Task ProduceUserRegisteredAsync(UserRegisteredMessage @event);
    }
}