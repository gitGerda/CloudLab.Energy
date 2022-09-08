using DataCollectionService.Test.Helpers;
using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Interfaces;
using kzmpCloudAPI.Services.BackgroundServices.XML80020_RabbitMQ_Consumer;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using Microsoft.Extensions.Logging;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static kzmpCloudAPI.Components.General.AppConsts;

namespace kzmpCloudAPI.Tests.ServicesTest
{
    public class APIRabbitConsumerTest
    {
        //------------------------------------------------------------------
        //HandleBrokerMessage
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
        public async void HandleBrokerMessage_OnInvoke_VerifyBrokerMessageToStringInvoke(byte[] message)
        {
            //----------
            var _persist_conn = new Mock<IRabbitMQPersistentConnection>();
            var _messages_handler = new Mock<IRabbitMessagesHandler>();
            var _logger = new Mock<ILogger<APIRabbitConsumer>>().Object;
            var _target = new APIRabbitConsumer(persistent_connection: _persist_conn.Object,
                messages_handler: _messages_handler.Object,
                def_queue_name: "",
                def_exchange_name: "",
                _logger);
            //--------------
            var _result = await _target.HandleBrokerMessage(message);
            //--------------
            _messages_handler.Verify(t => t.BrokerMessageToString(message));
        }
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
        public async void HandleBrokerMessage_OnInvoke_VerifyGetTypeOfBrokerMessageInvoke(byte[] message)
        {
            //----------
            var _persist_conn = new Mock<IRabbitMQPersistentConnection>();
            var _messages_handler = new Mock<IRabbitMessagesHandler>();
            _messages_handler.Setup(t => t.BrokerMessageToString(message)).Returns("");
            var _logger = new Mock<ILogger<APIRabbitConsumer>>().Object;

            var _target = new APIRabbitConsumer(persistent_connection: _persist_conn.Object,
                messages_handler: _messages_handler.Object,
                def_queue_name: "",
                def_exchange_name: "",
                _logger);
            //--------------
            var _result = await _target.HandleBrokerMessage(message);
            //--------------
            _messages_handler.Verify(t => t.GetTypeOfBrokerMessage(""));
        }
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
        public async void HandleBrokerMessage_OnSheduleLogTypeMessage_VerifyHandleSheduleLogTypeMessageInvoke(byte[] message)
        {
            //----------
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _persist_conn = new Mock<IRabbitMQPersistentConnection>();

            var _messages_handler = new Mock<IRabbitMessagesHandler>();
            _messages_handler.Setup(t => t.BrokerMessageToString(message)).Returns("");
            _messages_handler.Setup(t => t.GetTypeOfBrokerMessage("")).Returns(broker_messages_types.shedule_log_type);
            _messages_handler.Setup(t => t.DeserializeBrokerMessage<SheduleLog>("")).Returns(_shedule_log);
            var _logger = new Mock<ILogger<APIRabbitConsumer>>().Object;

            var _target = new APIRabbitConsumer(persistent_connection: _persist_conn.Object,
                messages_handler: _messages_handler.Object,
                def_queue_name: "",
                def_exchange_name: "", _logger);
            //--------------
            var _result = await _target.HandleBrokerMessage(message);
            //--------------
            _messages_handler.Verify(t => t.HandleSheduleLogTypeMessage(_shedule_log));
        }
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
        public async void HandleBrokerMessage_OnPowerProfileTypeMessage_VerifyHandlePowerProfileTypeMessageInvoke(byte[] message)
        {
            //----------
            var _power_profile_message = GeneralHelper._getPowerProfileBrokerMessage();
            var _persist_conn = new Mock<IRabbitMQPersistentConnection>();

            var _messages_handler = new Mock<IRabbitMessagesHandler>();
            _messages_handler.Setup(t => t.BrokerMessageToString(message)).Returns("");
            _messages_handler.Setup(t => t.GetTypeOfBrokerMessage("")).Returns(broker_messages_types.power_profiles_broker_message_type);
            _messages_handler.Setup(t => t.DeserializeBrokerMessage<PowerProfilesBrokerMessage>("")).Returns(_power_profile_message);
            var _logger = new Mock<ILogger<APIRabbitConsumer>>().Object;

            var _target = new APIRabbitConsumer(persistent_connection: _persist_conn.Object,
                messages_handler: _messages_handler.Object,
                def_queue_name: "",
                def_exchange_name: "", _logger);
            //--------------
            var _result = await _target.HandleBrokerMessage(message);
            //--------------
            _messages_handler.Verify(t => t.HandlePowerProfilesTypeMessage(_power_profile_message), Times.Once);
        }
        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
        public async void HandleBrokerMessage_OnBrokerTaskTypeMessage_VerifyHandleBrokerTaskTypeMessageInvoke(byte[] message)
        {
            //----------
            var _broker_task_message = GeneralHelper._getDefaultBrokerTaskMessageList().First();
            var _persist_conn = new Mock<IRabbitMQPersistentConnection>();

            var _messages_handler = new Mock<IRabbitMessagesHandler>();
            _messages_handler.Setup(t => t.BrokerMessageToString(message)).Returns("");
            _messages_handler.Setup(t => t.GetTypeOfBrokerMessage("")).Returns(broker_messages_types.broker_task_message_type);
            _messages_handler.Setup(t => t.DeserializeBrokerMessage<BrokerTaskMessage>("")).Returns(_broker_task_message);
            var _logger = new Mock<ILogger<APIRabbitConsumer>>().Object;

            var _target = new APIRabbitConsumer(persistent_connection: _persist_conn.Object,
                messages_handler: _messages_handler.Object,
                def_queue_name: "",
                def_exchange_name: "", _logger);
            //--------------
            var _result = await _target.HandleBrokerMessage(message);
            //--------------
            _messages_handler.Verify(t => t.HandleBrokerTaskTypeMessage(_broker_task_message), Times.Once);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
        public async void HandleBrokerMessage_OnEnergyResponseTypeMessage_VerifyHandleEnergyResponseTypeMessageInvoke(byte[] message)
        {
            //----------
            var _energy_response = GeneralHelper._getEnergyRecordResponse();
            var _persist_conn = new Mock<IRabbitMQPersistentConnection>();

            var _messages_handler = new Mock<IRabbitMessagesHandler>();
            _messages_handler.Setup(t => t.BrokerMessageToString(message)).Returns("");
            _messages_handler.Setup(t => t.GetTypeOfBrokerMessage("")).Returns(broker_messages_types.energy_response_message_type);
            _messages_handler.Setup(t => t.DeserializeBrokerMessage<EnergyRecordResponse>("")).Returns(_energy_response);
            var _logger = new Mock<ILogger<APIRabbitConsumer>>().Object;

            var _target = new APIRabbitConsumer(persistent_connection: _persist_conn.Object,
                messages_handler: _messages_handler.Object,
                def_queue_name: "",
                def_exchange_name: "", _logger);
            //--------------
            var _result = await _target.HandleBrokerMessage(message);
            //--------------
            _messages_handler.Verify(t => t.HandleEnergyResponseTypeMessage(_energy_response), Times.Once);
        }
    }
}
