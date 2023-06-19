namespace StreamingMicroservice.Services.Bus
{
    public class BusSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string QueueName { get; set; } = null!;
    }
}
