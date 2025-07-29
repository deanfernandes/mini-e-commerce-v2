using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Kafka.Contracts.Topics;

namespace EmailService.Services;

public class KafkaConsumerService
{
    private readonly IConfiguration _config;

    public KafkaConsumerService(IConfiguration config)
    {
        _config = config;
    }

    public void StartConsuming(Action<string> handleMessage)
    {
        var kafkaSection = _config.GetSection("KafkaSettings");

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSection["BootstrapServers"],
            GroupId = "email-service-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        consumer.Subscribe(Topics.UserRegistered);

        Console.WriteLine($"Subscribed to Kafka topic: {Topics.UserRegistered}");

        try
        {
            while (true)
            {
                var cr = consumer.Consume();
                Console.WriteLine($"Message received: {cr.Message.Value}");
                handleMessage(cr.Message.Value);
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }
}