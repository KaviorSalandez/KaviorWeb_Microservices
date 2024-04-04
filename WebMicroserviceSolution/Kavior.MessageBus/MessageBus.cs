using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kavior.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://kaviorweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aB7ntbb+/+XCH8r5COuHtsLODL6NmYhoV+ASbJ5JSeE=";
        public async Task PublicMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(), 
            };
            await sender.SendMessageAsync(finalMessage);    
            await client.DisposeAsync();    
        }
    }
}
