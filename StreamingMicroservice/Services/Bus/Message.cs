namespace StreamingMicroservice.Services.Bus
{
    public class Message<T>
    {
        public T Data { get; set; } = default!;
        public string Table { get; set; } = null!;
        public int Action { get; set; }
    }

    public enum MessageAction
    {
        Create = 1, 
        Update = 2,
        Delete = 3,
    }
}
