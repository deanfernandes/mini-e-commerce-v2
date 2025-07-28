namespace AuthService.Api.Services
{
    public interface IKafkaProducerService
    {
        Task ProduceUserRegisteredAsync(string message);
    }
}