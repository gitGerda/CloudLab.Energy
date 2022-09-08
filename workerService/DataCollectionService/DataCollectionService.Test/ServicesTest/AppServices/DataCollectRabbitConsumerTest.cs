using DataCollectionService.Components;
using DataCollectionService.Interfaces.IComponents;
using DataCollectionService.Interfaces.IServices.IAppServices;
using DataCollectionService.Services.AppServices;
using DataCollectionService.Test.Helpers;
using HangfireJobsToRabbitLibrary.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Test.ServicesTest.AppServices
{
    public class DataCollectRabbitConsumerTest
    {

        //--------------------------------------------------------------------------------------------------
        //ConsumerReceived
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })]
        public async void ConsumerReceived_OnInvoke_VerifyBasicAckOkAndCloseConnectionInvoke(byte[] message)
        {
            //------
            var _def_deserial_obj = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _def_deserial_str = JsonConvert.SerializeObject(_def_deserial_obj);
            var _events_args = new BasicDeliverEventArgs();

            var _indic_reader = new Mock<IIndicationsReader>();
            _indic_reader.Setup(t => t.BrokerMessageToString(message)).Returns(_def_deserial_str);
            _indic_reader.Setup(t => t.GetTypeOfBrokerMessage(_def_deserial_str)).Returns(AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE);

            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _logger = new Mock<ILogger<DataCollectRabbitConsumer>>().Object;
            var _rabbit_messaging_ext = new Mock<IRabbitMessagingExtension>();

            var _service = new DataCollectRabbitConsumer(_indic_reader.Object, _rabbit_conn.Object, "", "", _logger, _rabbit_messaging_ext.Object);
            //-------
            await _service.ConsumerReceived(new object(), _events_args);
            //------
            _rabbit_messaging_ext.Verify(t => t.BasicAckOkAndCloseConnection(_service, _events_args));
        }
        //--------------------------------------------------------------------------------------------------
        //HandleBrokerMessage   
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })]
        public async void HandleBrokerMessage_OnInvoke_VerifyDeserializeBrokerMessageInvoke(byte[] message)
        {
            //---
            var _def_deserial_obj = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _def_deserial_str = JsonConvert.SerializeObject(_def_deserial_obj);

            var _indic_reader = new Mock<IIndicationsReader>();
            _indic_reader.Setup(t => t.BrokerMessageToString(message)).Returns(_def_deserial_str);
            _indic_reader.Setup(t => t.GetTypeOfBrokerMessage(_def_deserial_str)).Returns(AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE);

            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _logger = new Mock<ILogger<DataCollectRabbitConsumer>>().Object;
            var _rabbit_messaging_ext = new Mock<IRabbitMessagingExtension>().Object;
            var _service = new DataCollectRabbitConsumer(indic_reader: _indic_reader.Object, rabbit_connection: _rabbit_conn.Object, queue_name: "",
                exchange_name: "", _logger, _rabbit_messaging_ext);
            //---
            var result = await _service.HandleBrokerMessage(message);
            //---
            _indic_reader.Verify(t => t.BrokerMessageToString(message), Times.Once);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })]
        public async void HandleBrokerMessage_OnInvoke_VerifyGetTypeOfBrokerMessageInvoke(byte[] message)
        {
            //---
            var _def_deserial_obj = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _def_deserial_str = JsonConvert.SerializeObject(_def_deserial_obj);

            var _indic_reader = new Mock<IIndicationsReader>();
            _indic_reader.Setup(t => t.BrokerMessageToString(message)).Returns(_def_deserial_str);
            _indic_reader.Setup(t => t.GetTypeOfBrokerMessage(_def_deserial_str)).Returns(AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE);

            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _logger = new Mock<ILogger<DataCollectRabbitConsumer>>().Object;
            var _rabbit_messaging_ext = new Mock<IRabbitMessagingExtension>().Object;

            var _service = new DataCollectRabbitConsumer(indic_reader: _indic_reader.Object, rabbit_connection: _rabbit_conn.Object, queue_name: "",
                exchange_name: "", _logger, _rabbit_messaging_ext);
            //---
            var result = await _service.HandleBrokerMessage(message);
            //---
            _indic_reader.Verify(t => t.GetTypeOfBrokerMessage(_def_deserial_str), Times.Once);
        }
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })]
        public async void HandleBrokerMessage_OnListBrokerTaskMessageTypeOfBrokerMessage_VerifyGetIndicationsInvoke(byte[] message)
        {
            //---
            var _def_deserial_obj = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _def_deserial_str = JsonConvert.SerializeObject(_def_deserial_obj);

            var _indic_reader = new Mock<IIndicationsReader>();
            _indic_reader.Setup(t => t.BrokerMessageToString(message)).Returns(_def_deserial_str);
            _indic_reader.Setup(t => t.GetTypeOfBrokerMessage(_def_deserial_str)).Returns(AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE);
            _indic_reader.Setup(t => t.DeserializeBrokerMessage<List<BrokerTaskMessage>>(_def_deserial_str)).Returns(_def_deserial_obj);

            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _logger = new Mock<ILogger<DataCollectRabbitConsumer>>().Object;
            var _rabbit_messaging_ext = new Mock<IRabbitMessagingExtension>().Object;

            var _service = new DataCollectRabbitConsumer(indic_reader: _indic_reader.Object, rabbit_connection: _rabbit_conn.Object, queue_name: "",
                exchange_name: "", _logger, _rabbit_messaging_ext);
            //---
            var result = await _service.HandleBrokerMessage(message);
            //---
            _indic_reader.Verify(t => t.GetIndications(_def_deserial_obj), Times.Once);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })]
        public async void HandleBrokerMessage_OnPortConfMessage_VerifyChangeComPortNameInvoke(byte[] message)
        {
            //---
            var _def_deserial_obj = new PortConfiguration { PortName = "COM1" };
            var _def_deserial_str = JsonConvert.SerializeObject(_def_deserial_obj);

            var _indic_reader = new Mock<IIndicationsReader>();
            _indic_reader.Setup(t => t.BrokerMessageToString(message)).Returns(_def_deserial_str);
            _indic_reader.Setup(t => t.GetTypeOfBrokerMessage(_def_deserial_str)).Returns(AppConsts.PORT_CONFIGURATION_TYPE);
            _indic_reader.Setup(t => t.DeserializeBrokerMessage<PortConfiguration>(_def_deserial_str)).Returns(_def_deserial_obj);

            var _rabbit_conn = new Mock<IRabbitMQPersistentConnection>();
            var _logger = new Mock<ILogger<DataCollectRabbitConsumer>>().Object;
            var _rabbit_messaging_ext = new Mock<IRabbitMessagingExtension>().Object;

            var _service = new DataCollectRabbitConsumer(indic_reader: _indic_reader.Object, rabbit_connection: _rabbit_conn.Object, queue_name: "",
                exchange_name: "", _logger, _rabbit_messaging_ext);
            //---
            var result = await _service.HandleBrokerMessage(message);
            //---
            _indic_reader.Verify(t => t.ChangeComPortName("COM1"));
        }

    }
}
