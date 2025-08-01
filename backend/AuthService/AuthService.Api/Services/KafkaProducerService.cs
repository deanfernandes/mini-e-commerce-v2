using System.Text.Json;
using Confluent.Kafka;
using Kafka.Contracts.Messages;
using Kafka.Contracts.Topics;

namespace AuthService.Api.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration config)
        {
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = config["KafkaSettings:BootstrapServers"]
            };
            _producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
        }

        public async Task ProduceUserRegisteredAsync(UserRegisteredMessage @event)
        {
            var message = JsonSerializer.Serialize(@event);
            await _producer.ProduceAsync(Topics.UserRegistered, new Message<Null, string> { Value = message });
        }
    }
}