using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Newtonsoft.Json;
using HangfireJobsToRabbitLibrary.Jobs.IndicationsReading;
using HangfireJobsToRabbitLibrary.Models;

namespace kzmpCloudAPI.Tests.LibrariesTest.HangfireJobsToRabbit
{
    public class HangfireRabbitJobsTest
    {
        [Fact]
        public async void ReccuringJobAddOrUpdate_OnEmptyRequestStr_FalseResult()
        {
            //arrange
            JobCreateSettings _settings = new JobCreateSettings()
            {
                last_datetime_request = "",
                rabbit_server_address = "http://localhost"
            };
            var _message_list = new List<BrokerTaskMessage>() { new BrokerTaskMessage() };
            var _hangfire_rabbit_job_mock = new Mock<HangfireRabbitJobs>();
            _hangfire_rabbit_job_mock.Setup(ex => ex._AddOrUpdate(_settings, _message_list)).Returns(true);
            //act
            var result = await _hangfire_rabbit_job_mock.Object.ReccuringJobAddOrUpdate(_settings, _message_list);
            //assert
            Assert.False(result);
        }
        [Fact]
        public async void ReccuringJobAddOrUpdate_OnEmptyMessageList_FalseResult()
        {
            //arrange
            JobCreateSettings _settings = new JobCreateSettings()
            {
                last_datetime_request = "http://localhost",
                rabbit_server_address = "http://localhost"
            };
            var _message_list = new List<BrokerTaskMessage>();
            var _hangfire_rabbit_job_mock = new Mock<HangfireRabbitJobs>();
            _hangfire_rabbit_job_mock.Setup(ex => ex._AddOrUpdate(_settings, _message_list)).Returns(true);
            //act
            var result = await _hangfire_rabbit_job_mock.Object.ReccuringJobAddOrUpdate(_settings, _message_list);
            //assert
            Assert.False(result);
        }
        [Fact]
        public async void ReccuringJobAddOrUpdate_OnEmptyRabbitServerAddress_FalseResult()
        {
            JobCreateSettings _settings = new JobCreateSettings()
            {
                last_datetime_request = "http://localhost",
                rabbit_server_address = ""
            };
            var _message_list = new List<BrokerTaskMessage>() { new BrokerTaskMessage() };
            var _hangfire_rabbit_job_mock = new Mock<HangfireRabbitJobs>();
            _hangfire_rabbit_job_mock.Setup(ex => ex._AddOrUpdate(_settings, _message_list)).Returns(true);
            //act
            var result = await _hangfire_rabbit_job_mock.Object.ReccuringJobAddOrUpdate(_settings, _message_list);
            //assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(new byte[2] { 0x01, 0x02 }, "http://localhost")]
        public async void ReProcessingRabbitMessage_OnPrevJsonMessageNotTypeListBrokerTaskMessage_ExceptionResult(byte[] prev_json_mes, string request_str)
        {
            //arrange
            var _hangfire_rabbit_job_mock = new Mock<HangfireRabbitJobs>();
            _hangfire_rabbit_job_mock.Setup(ex => ex.ConvertMessageToObject(prev_json_mes)).Returns(() => null);

            //assert 
            await Assert.ThrowsAsync<Exception>(() => _hangfire_rabbit_job_mock.Object.ReProcessingRabbitMessage(prev_json_mes, request_str));
        }

        [Fact]
        public async void ReProcessingRabbitMessage_OnEmptyRequestStr_ExceptionResult()
        {
            //arrange
            var _prev_json_mes = _GetValidTestRabbitMessageHelper();
            var _hangfire_obj = new HangfireRabbitJobs();

            //assert
            await Assert.ThrowsAsync<Exception>(() => _hangfire_obj.ReProcessingRabbitMessage(_prev_json_mes, ""));
        }

        [Theory]
        [InlineData("http://localhost")]
        public async void ReProcessingRabbitMessage_OnNullReturnInGetLastIndicationDateTime_SameMessageList(string request)
        {
            //arrange
            var _message_list = new List<BrokerTaskMessage>()
            {
                new BrokerTaskMessage()
                {
                    communic_interface="test",
                    last_indication_datetime=""
                }
            };
            byte[] _prev_mes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_message_list));

            var _hangfire_obj_mock = new Mock<HangfireRabbitJobs>();
            _hangfire_obj_mock.Setup(ex => ex.ConvertMessageToObject(_prev_mes)).Returns(_message_list);
            _hangfire_obj_mock.Setup(ex => ex.GetLastIndicationDateTime(new System.Net.Http.HttpClient(), request)).Returns(() => null);

            //act
            var result = await _hangfire_obj_mock.Object.ReProcessingRabbitMessage(_prev_mes, request);

            //assert
            Assert.Equal(_prev_mes, result);

        }

        private byte[] _GetValidTestRabbitMessageHelper()
        {
            List<BrokerTaskMessage> _message_list = new List<BrokerTaskMessage>() { new BrokerTaskMessage() };
            byte[] _prev_json_mes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_message_list));
            return _prev_json_mes;
        }
    }
}
