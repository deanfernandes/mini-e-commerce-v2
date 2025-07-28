using Confluent.Kafka;

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
            _topic = config["KafkaSettings:TopicUserRegistered"];
            _producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
        }

        public async Task ProduceUserRegisteredAsync(string message)
        {
            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
        }
    }
}