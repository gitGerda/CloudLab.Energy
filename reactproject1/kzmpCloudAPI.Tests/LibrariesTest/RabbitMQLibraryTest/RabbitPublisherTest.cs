using kzmpCloudAPI.Services.RabbitMQService;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.LibrariesTest.RabbitMQLibraryTest
{
    public class RabbitPublisherTest
    {
        [Theory]
        [InlineData("test_queue", "test_exchange", new byte[3] { 0x02, 0x03, 0x05 })]
        public void PublishMessage_OnInvoke_VerifyIsConnectedPropGet(string def_queue_name, string def_exchange_name, byte[] msg)
        {
            //----
            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _conf = new Mock<IConfiguration>().Object;
            var _basic_props = new Mock<IBasicProperties>().Object;
            var _model = new Mock<IModel>();
            _model.Setup(t => t.CreateBasicProperties()).Returns(_basic_props);

            var _publisher = new Mock<RabbitPublisher>(_rabbit_conn.Object, def_exchange_name, def_queue_name);

            _rabbit_conn.SetupGet(t => t.IsConnected).Returns(true);
            _rabbit_conn.Setup(t => t.CreateModel()).Returns(_model.Object);
            //----
            _publisher.Object.PublishMessage(_publisher.Object.publisher_channel, def_exchange_name, def_queue_name, msg);

            //----
            _rabbit_conn.VerifyGet(t => t.IsConnected);
        }
        [Theory]
        [InlineData("test_exchange", "test_queue", new byte[3] { 0x02, 0x03, 0x05 })]
        public void PublishMessage_OnInvoke_VerifyDeclareDefaultExchangeInvoke(string def_exchange, string queue_name, byte[] msg)
        {
            //----
            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _conf = new Mock<IConfiguration>().Object;
            var _model = new Mock<IModel>().Object;
            var _publisher = new Mock<RabbitPublisher>(_rabbit_conn.Object, def_exchange, queue_name);
            _rabbit_conn.SetupGet(t => t.IsConnected).Returns(true);
            _rabbit_conn.Setup(t => t.CreateModel()).Returns(_model);
            //----
            _publisher.Object.PublishMessage(_publisher.Object.publisher_channel, def_exchange, queue_name, msg);

            //----
            _publisher.Verify(t => t.DeclareExchange(_model, def_exchange));
        }

        /*        [Theory]
                [InlineData("test_queue", "test_exchange", new byte[3] { 0x02, 0x03, 0x05 })]
                public void PublishMessage_OnInvoke_VerifyDeclareAndBindQueueInvoke(string queue_name, string exchange_name, byte[] msg)
                {
                    //----
                    var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
                    var _conf = new Mock<IConfiguration>().Object;
                    var _model = new Mock<IModel>().Object;
                    var _publisher = new Mock<RabbitPublisher>(_rabbit_conn.Object, exchange_name, queue_name);
                    _rabbit_conn.SetupGet(t => t.IsConnected).Returns(true);
                    _rabbit_conn.Setup(t => t.CreateModel()).Returns(_model);
                    //----
                    _publisher.Object.PublishMessage(_publisher.Object.publisher_channel, exchange_name, queue_name, msg);

                    //----
                    _publisher.Verify(t => t.DeclareAndBindQueue(_model, queue_name, exchange_name));
                }*/
        /*        [Theory]
                [InlineData("test_queue", "test_exchange", new byte[3] { 0x02, 0x03, 0x05 })]
                public void PublishMessage_OnInvoke_VerifySendMessageInvoke(string queue_name, string exchange_name, byte[] msg)
                {
                    //----
                    var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
                    var _conf = new Mock<IConfiguration>().Object;
                    var _model = new Mock<IModel>().Object;
                    var _publisher = new Mock<RabbitPublisher>(_rabbit_conn.Object, exchange_name, queue_name);
                    _rabbit_conn.SetupGet(t => t.IsConnected).Returns(true);
                    _rabbit_conn.Setup(t => t.CreateModel()).Returns(_model);
                    //----
                    _publisher.Object.PublishMessage(_publisher.Object.publisher_channel, exchange_name, queue_name, msg);

                    //----
                    _publisher.Verify(t => t.SendMessage(_model, msg, exchange_name, queue_name));
                }*/
    }
}
