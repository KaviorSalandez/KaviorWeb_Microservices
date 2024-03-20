using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kavior.MessageBus
{
    public interface IMessageBus
    {
        Task PublicMessage(object message, string topic_queue_Name);
    }
}
