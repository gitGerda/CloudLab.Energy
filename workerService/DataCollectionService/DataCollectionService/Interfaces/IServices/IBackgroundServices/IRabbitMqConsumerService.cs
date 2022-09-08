using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Interfaces.IServices.IBackgroundServices
{
    public interface IRabbitMqConsumerService : IHostedService
    {
        public Task ExecuteAsync(CancellationToken token);
        public void CloseService(CancellationTokenSource source);
    }
}
