using Microsoft.Extensions.Configuration;
using Kafka.Contracts.Messages;
using EmailService.Services;

namespace EmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var emailService = new EmailService(config);
            var kafkaService = new KafkaConsumerService(config);

            kafkaService.StartConsuming(messageJson =>
            {
                try
                {
                    var @event = System.Text.Json.JsonSerializer.Deserialize<UserRegisteredMessage>(messageJson);
                    if (@event is not null && !string.IsNullOrWhiteSpace(@event.Email) && !string.IsNullOrWhiteSpace(@event.Token))
                    {
                        emailService.SendConfirmationEmail(@event.Email, @event.Token);
                    }
                    else
                    {
                        Console.WriteLine("Invalid user event data.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process message: {ex.Message}");
                }
            });
        }
    }
}