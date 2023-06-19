namespace StreamingMicroservice.Services.Bus
{
    public interface IBusSender
    {
        public Task SendMessage<T>(Message<T> message);
    }
}
