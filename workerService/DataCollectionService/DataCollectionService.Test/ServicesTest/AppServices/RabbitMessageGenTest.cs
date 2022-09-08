using DataCollectionService.Services.AppServices;
using DataCollectionService.Test.Helpers;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Test.ServicesTest.AppServices
{
    public class RabbitMessageGenTest
    {
        /*        //----------------------------------------------------------------------
                //PublishEnergyIndicationsMessage
                [Fact]
                public void PublishEnergyIndicationsMessage_OnInvoke_VerifyGetBytesFromSheduleLogTypeInvoke()
                {
                    //---------
                    var _response = GeneralHelper._getEnergyRecordResponse();
                    var _rabbit_publisher = new Mock<IRabbitPublisher>().Object;
                    var _target = new Mock<RabbitMessageGen>(_rabbit_publisher);
                    //-------
                    var _result = _target.Object.PublishEnergyIndicationsMessage(_response);
                    //-------
                    _target.Verify(t => t.GetBytesFromMessage(_response));
                }
                //-----------------------------------------------------------------------
                //PublishSheduleLogMessage
                [Fact]
                public void PublishSheduleLogMessage_OnInvoke_VerifyGetBytesFromSheduleLogTypeInvoke()
                {
                    //----
                    var _shedule_log = GeneralHelper._getDefaultSheduleLog();
                    var _rabbit_publisher = new Mock<IRabbitPublisher>();
                    var _target_obj = new Mock<RabbitMessageGen>(_rabbit_publisher.Object);
                    _target_obj.Setup(t => t.GetBytesFromMessage(_shedule_log)).Returns(new byte[] { 0x01, 0x02, 0x03, 0x04 });

                    //----
                    var result = _target_obj.Object.PublishSheduleLogMessage(_shedule_log);

                    //----
                    _target_obj.Verify(t => t.GetBytesFromMessage(_shedule_log));
                }

                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishSheduleLogMessage_OnInvoke_VerifyPublishMessageInvoke(byte[] message)
                {
                    //----
                    var _shedule_log = GeneralHelper._getDefaultSheduleLog();
                    var _rabbit_publisher = new Mock<IRabbitPublisher>();
                    var _target_obj = new Mock<RabbitMessageGen>(_rabbit_publisher.Object);
                    _target_obj.Setup(t => t.GetBytesFromMessage(_shedule_log)).Returns(message);

                    //----
                    var result = _target_obj.Object.PublishSheduleLogMessage(_shedule_log);

                    //----
                    _rabbit_publisher.Verify(t => t.PublishMessage(_rabbit_publisher.Object.publisher_channel, _rabbit_publisher.Object.def_exchange_name, _rabbit_publisher.Object.def_queue_name, message));
                }

                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishSheduleLogMessage_OnThrowOfPublishMessage_VerifyPublishMessageInvoke(byte[] message)
                {
                    //----
                    var _shedule_log = GeneralHelper._getDefaultSheduleLog();

                    var _rabbit_publisher = new Mock<IRabbitPublisher>();
                    _rabbit_publisher.Setup(t => t.PublishMessage(_rabbit_publisher.Object.publisher_channel, _rabbit_publisher.Object.def_exchange_name, _rabbit_publisher.Object.def_queue_name, message)).Throws<Exception>();

                    var _target_obj = new Mock<RabbitMessageGen>(_rabbit_publisher.Object);
                    _target_obj.Setup(t => t.GetBytesFromMessage(_shedule_log)).Returns(message);
                    //----
                    var result = _target_obj.Object.PublishSheduleLogMessage(_shedule_log);
                    //----
                    Assert.False(result);
                }
                //--------------------------------------------------------------------------------------
                //PublishFailedBrokerTaskTypeMessage

                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishFailedBrokerTaskTypeMessage_OnInvoke_VerifyGetBytesFromBrokerTaskMessageTypeInvoke(byte[] message)
                {
                    //----
                    var _broker_task_message = GeneralHelper._getDefaultBrokerTaskMessageList().First();
                    var _rabbit_publisher = new Mock<IRabbitPublisher>();
                    var _target_obj = new Mock<RabbitMessageGen>(_rabbit_publisher.Object);
                    _target_obj.Setup(t => t.GetBytesFromMessage(_broker_task_message)).Returns(message);
                    //-----
                    var result = _target_obj.Object.PublishFailedBrokerTaskTypeMessage(_broker_task_message);
                    //-----
                    _target_obj.Verify(t => t.GetBytesFromMessage(_broker_task_message), Times.Once);
                }
                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishFailedBrokerTaskTypeMessage_OnInvoke_VerifyPublishMessageInvoke(byte[] message)
                {
                    //----
                    var _broker_task_message = GeneralHelper._getDefaultBrokerTaskMessageList().First();
                    var _rabbit_publisher = new Mock<IRabbitPublisher>();
                    var _target_obj = new Mock<RabbitMessageGen>(_rabbit_publisher.Object);
                    _target_obj.Setup(t => t.GetBytesFromMessage(_broker_task_message)).Returns(message);
                    //-----
                    var result = _target_obj.Object.PublishFailedBrokerTaskTypeMessage(_broker_task_message);
                    //-----
                    _rabbit_publisher.Verify(t => t.PublishMessage(_rabbit_publisher.Object.publisher_channel, _rabbit_publisher.Object.def_exchange_name, _rabbit_publisher.Object.def_queue_name, message));
                }
                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishFailedBrokerTaskTypeMessage_OnThrowOfPublishMessage_VerifyPublishMessageInvoke(byte[] message)
                {
                    //----
                    var _broker_task_message = GeneralHelper._getDefaultBrokerTaskMessageList().First();

                    var _rabbit_publisher = new Mock<IRabbitPublisher>();
                    _rabbit_publisher.Setup(t => t.PublishMessage(_rabbit_publisher.Object.publisher_channel, _rabbit_publisher.Object.def_exchange_name, _rabbit_publisher.Object.def_queue_name, message)).Throws<Exception>();

                    var _target_obj = new Mock<RabbitMessageGen>(_rabbit_publisher.Object);
                    _target_obj.Setup(t => t.GetBytesFromMessage(_broker_task_message)).Returns(message);
                    //-----
                    var result = _target_obj.Object.PublishFailedBrokerTaskTypeMessage(_broker_task_message);
                    //-----
                    Assert.False(result);
                }
                //---------------------------------------------------------------------------------------------------------------------------
                //PublishPowerProfilesMessage
                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishPowerProfilesMessage_OnInvoke_VerifyGetBytesFromMessageInvoke(byte[] message)
                {
                    //----------------
                    var _message = GeneralHelper._getPowerProfileBrokerMessage();
                    var _publisher = new Mock<IRabbitPublisher>().Object;

                    var _target = new Mock<RabbitMessageGen>(_publisher);
                    _target.Setup(t => t.GetBytesFromMessage(_message)).Returns(message);
                    //----------------
                    _target.Object.PublishPowerProfilesMessage(_message);
                    //----------------
                    _target.Verify(t => t.GetBytesFromMessage(_message));
                }
                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishPowerProfilesMessage_OnInvoke_VerifyPublishMessageInvoke(byte[] message)
                {
                    //----------------
                    var _message = GeneralHelper._getPowerProfileBrokerMessage();
                    var _publisher = new Mock<IRabbitPublisher>();

                    var _target = new Mock<RabbitMessageGen>(_publisher.Object);
                    _target.Setup(t => t.GetBytesFromMessage(_message)).Returns(message);
                    //----------------
                    _target.Object.PublishPowerProfilesMessage(_message);
                    //----------------
                    _publisher.Verify(t => t.PublishMessage(_publisher.Object.publisher_channel, _publisher.Object.def_exchange_name, _publisher.Object.def_queue_name, message));
                }
                [Theory]
                [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04 })]
                public void PublishPowerProfilesMessage_OnThrowOfPublishMessage_VerifyPublishMessageInvoke(byte[] message)
                {
                    //----------------
                    var _message = GeneralHelper._getPowerProfileBrokerMessage();
                    var _publisher = new Mock<IRabbitPublisher>();
                    _publisher.Setup(t => t.PublishMessage(_publisher.Object.publisher_channel, _publisher.Object.def_exchange_name, _publisher.Object.def_queue_name, message)).Throws<Exception>();

                    var _target = new Mock<RabbitMessageGen>(_publisher.Object);
                    _target.Setup(t => t.GetBytesFromMessage(_message)).Returns(message);
                    //----------------
                    var _result = _target.Object.PublishPowerProfilesMessage(_message);
                    //----------------
                    Assert.False(_result);
                }
        */
    }
}
