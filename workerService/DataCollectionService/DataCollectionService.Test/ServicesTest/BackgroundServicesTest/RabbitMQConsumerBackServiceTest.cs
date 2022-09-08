using DataCollectionService.Services.AppServices;
using DataCollectionService.Services.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.Interfaces;

namespace DataCollectionService.Test.ServicesTest.BackgroundServicesTest
{
    public class RabbitMQConsumerBackServiceTest
    {
        [Fact]
        public void StartAsync_OnInvoke_VerifyExecuteAsyncInvoke()
        {
            //-----
            var _cancellation_source = new Mock<CancellationTokenSource>().Object;
            var _cancelation_token = _cancellation_source.Token;
            var _logger = new Mock<ILogger<RabbitMqConsumerBackService>>();
            var _rabbit_consumer = new Mock<IRabbitConsumer>();

            var _service = new Mock<RabbitMqConsumerBackService>(_logger.Object, _cancellation_source, _cancelation_token,_rabbit_consumer.Object);
            _service.Setup(t => t.ExecuteAsync(_cancelation_token)).Returns(Task.CompletedTask);

            //----
            var result = _service.Object.StartAsync(_cancelation_token);

            //----
            _service.Verify(t => t.ExecuteAsync(_cancelation_token));
        }

        [Fact]
        public void StopAsync_OnInvoke_VerifyCancelationSourceCancelInvoke()
        {
            //-----
            var _cancellation_source = new Mock<CancellationTokenSource>();
            var _cancelation_token = _cancellation_source.Object.Token;
            var _logger = new Mock<ILogger<RabbitMqConsumerBackService>>();
            var _rabbit_consumer = new Mock<IRabbitConsumer>();

            var _service = new Mock<RabbitMqConsumerBackService>(_logger.Object, _cancellation_source.Object, _cancelation_token,_rabbit_consumer.Object);

            //----
            var result = _service.Object.StopAsync(_cancelation_token);

            //----
            _service.Verify(t => t.CloseService(_cancellation_source.Object));
        }
    }
}
