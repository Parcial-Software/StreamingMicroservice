
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Text;

namespace StreamingMicroservice.Services.Bus
{
    public class BusSender : IBusSender 
    {
        private readonly ServiceBusSender _sender;
        public BusSender(IOptions<BusSettings> options)
        {
            var settings = options.Value;
            _sender = new ServiceBusClient(settings.ConnectionString).CreateSender(settings.QueueName);
        }

        public async Task SendMessage<T>(Message<T> message)
        {

            var json = JsonConvert.SerializeObject(message);
            var data = new ServiceBusMessage(Encoding.UTF8.GetBytes(json));

            try
            {
                await _sender.SendMessageAsync(data);
            }
            finally
            {
                await _sender.DisposeAsync();
            }
        }
    }
}
