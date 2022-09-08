using DataCollectionService.Test.Helpers;
using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Services;
using kzmpCloudAPI.Tests.Fixtures;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static kzmpCloudAPI.Components.General.AppConsts;

namespace kzmpCloudAPI.Tests.ServicesTest
{
    public class RabbitMessagesHandlerTest : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture _fixture
        {
            get;
        }
        public RabbitMessagesHandlerTest(KzmpEnergyTestingDbFixture fixture)
        {
            _fixture = fixture;
        }
        //-------------------------------------------
        //HandleEnergyResponseTypeMessage
        [Fact]
        public void HandleEnergyResponseTypeMessage_OnInvoke_TrueResult()
        {
            //-----------------
            var _message = GeneralHelper._getEnergyRecordResponse();
            var _conf = new Mock<IConfiguration>();
            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            _target.Setup(t => t._GetCurrentDateTime()).Returns(DateTime.Now);
            //-----------------
            var _result = _target.Object.HandleEnergyResponseTypeMessage(_message);
            //-----------------
            Assert.True(_result);
        }
        [Fact]
        public void HandleEnergyResponseTypeMessage_OnExistingRecord_TrueResult()
        {
            //-------------------------
            var _database = _fixture.CreateContext();
            var _existing_record = _database.EnergyTables.FirstOrDefault();
            EnergyRecordResponse _message = new EnergyRecordResponse()
            {
                meter_id = _existing_record.MeterId,
                MonthNumber = Convert.ToInt32(_existing_record.Month),
                EndValue = 3860,
                MeterAddress = _existing_record.Address ?? -1,
                StartValue = 3560,
                TotalValue = 300,
                Year = _existing_record.Year,
                Logs = new Queue<Logs>(),
                shedule_id=0
            };
            var _conf = new Mock<IConfiguration>();
            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            _target.Setup(t => t._GetCurrentDateTime()).Returns(DateTime.Now);
            //-----------------
            var _result = _target.Object.HandleEnergyResponseTypeMessage(_message);
            //-----------------
            var _database2 = _fixture.CreateContext();

            Assert.True(_result);
            var _record_count = _database2.EnergyTables.Where(t => t.MeterId == _existing_record.MeterId && t.Month == _existing_record.Month && t.Year == _existing_record.Year).Count();
            Assert.Equal(1, _record_count);

            var _changed_record = _database2.EnergyTables.Where(t => t.MeterId == _existing_record.MeterId && t.Month == _existing_record.Month && t.Year == _existing_record.Year).Select(t => t).First();
            Assert.Equal(_message.StartValue.ToString(), _changed_record.StartValue);
            Assert.Equal(_message.EndValue.ToString(), _changed_record.EndValue);
            Assert.Equal(_message.TotalValue.ToString(), _changed_record.Total);
        }
        //-------------------------------------------
        //HandleBrokerTaskTypeMessage
        [Fact]
        public void HandleBrokerTaskTypeMessage_OnInvoke_TrueResult()
        {
            //-----------
            var _message = GeneralHelper._getDefaultBrokerTaskMessageList().First();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //------------
            var _result = _target.Object.HandleBrokerTaskTypeMessage(_message);
            //-----------
            Assert.True(_result);
        }
        [Fact]
        public void HandleBrokerTaskTypeMessage_OnInvalidStartDate_FalseResult()
        {
            //-----------
            var _message = GeneralHelper._getDefaultBrokerTaskMessageList().First();
            _message.start_date = "invalid_date";
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //------------
            var _result = _target.Object.HandleBrokerTaskTypeMessage(_message);
            //-----------
            Assert.False(_result);
        }

        //--------------------------------------------
        //HandlePowerProfilesTypeMessage
        [Fact]
        public void HandlePowerProfilesTypeMessage_OnNullMeterAddress_ReturnFalse()
        {
            //----------------------
            var _message = GeneralHelper._getPowerProfileBrokerMessage();
            _message.meter_id = -1;
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //------------------------
            var _result = _target.Object.HandlePowerProfilesTypeMessage(_message);
            //------------------------
            Assert.False(_result);
        }
        [Theory]
        [InlineData(4)]
        public void HandlePowerProfilesTypeMessage_OnInvoke_ReturnTrue(int meter_id)
        {
            //------------------
            var _message = GeneralHelper._getPowerProfileBrokerMessage();
            _message.meter_id = meter_id;
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //------------------------
            var _result = _target.Object.HandlePowerProfilesTypeMessage(_message);
            //------------------------
            Assert.True(_result);

        }
        [Theory]
        [InlineData(4)]
        public void HandlePowerProfilesTypeMessage_OnAttemptToAddExistingRecord_NoDuplicateEntriesInDb(int meter_id)
        {
            //------------------
            var _database = _fixture.CreateContext();
            var _meter_address = KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[meter_id];

            var _existing_record = _database.PowerProfileMs.Where(t => t.Address == _meter_address).Select(t => t).First();

            var _message = GeneralHelper._getPowerProfileBrokerMessage();
            _message.meter_id = meter_id;
            var _record = new PowerProfileRecord()
            {
                Pminus = _existing_record.Pminus,
                Pplus = _existing_record.Pplus,
                Qminus = _existing_record.Qminus,
                Qplus = _existing_record.Qplus,
                RecordDate = DateTime.Parse($"{_existing_record.Date.ToShortDateString()} {_existing_record.Time}")
            };
            _message.Records.Enqueue(_record);

            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_database);
            //------------------------
            var _result = _target.Object.HandlePowerProfilesTypeMessage(_message);
            //------------------------
            var _record_count = _database.PowerProfileMs.Where(t => t.Address == _meter_address && t.Date == _existing_record.Date && t.Time == _existing_record.Time).Count();
            Assert.Equal(1, _record_count);
        }
        //---------------------------------------------
        //HandleSheduleLogTypeMessage
        [Fact]
        public void HandleSheduleLogTypeMessage_OnInvoke_ReturnTrue()
        {
            //-----------------
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //----------------
            var _result = _target.Object.HandleSheduleLogTypeMessage(_shedule_log);
            //----------------
            Assert.True(_result);
        }
        //---------------------------------------------
        //_GetMeterAddress
        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public void _GetMeterAddress_OnValidMeterId_ReturnIntResult(int meter_id, int address)
        {
            //-----------------
            var _database = _fixture.CreateContext();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //----------------
            var _result = _target.Object._GetMeterAddress(meter_id);
            //----------------
            Assert.Equal(address, _result);
        }
        [Theory]
        [InlineData(-1)]
        public void _GetMeterAddress_OnInValidMeterId_ReturnNull(int meter_id)
        {
            //-----------------
            var _database = _fixture.CreateContext();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //----------------
            var _result = _target.Object._GetMeterAddress(meter_id);
            //----------------
            Assert.Null(_result);
        }

        //----------------------------------------------
        //BrokerMessageToString
        [Fact]
        public void BrokerMessageToString_OnInvoke_ValidMessageResult()
        {
            //------
            const string _message = "test";
            byte[] _message_bytes = Encoding.UTF8.GetBytes(_message);
            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //------
            var _result = _target.Object.BrokerMessageToString(_message_bytes);
            //-------
            Assert.Equal(_message, _result);
        }
        //----------------------------------------------
        //DeserializeBrokerMessage
        [Fact]
        public void DeserializeBrokerMessage_OnInvoke_ValidObjectResult()
        {
            //-------------
            var _broker_mes_obj = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _broker_mes_str = JsonConvert.SerializeObject(_broker_mes_obj);

            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //--------------
            var _result = _target.Object.DeserializeBrokerMessage<List<BrokerTaskMessage>>(_broker_mes_str);
            //--------------
            for (int i = 0; i < _result.Count; i++)
            {
                Assert.Equal(_broker_mes_obj[i].meter_address, _result[i].meter_address);
                Assert.Equal(_broker_mes_obj[i].meter_type, _result[i].meter_type);
                Assert.Equal(_broker_mes_obj[i].sim_number, _result[i].sim_number);
                Assert.Equal(_broker_mes_obj[i].meter_id, _result[i].meter_id);
                Assert.Equal(_broker_mes_obj[i].shedule_id, _result[i].shedule_id);
            }
        }
        //--------------------------------------------------
        //GetTypeOfBrokerMessage
        [Fact]
        public void GetTypeOfBrokerMessage_OnSheduleLogMessageType_ReturnSheduleLogTypeOfEnum()
        {
            //----------------
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _message = JsonConvert.SerializeObject(_shedule_log);

            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //-------------
            var _result = _target.Object.GetTypeOfBrokerMessage(_message);
            //------------
            Assert.Equal(broker_messages_types.shedule_log_type, _result);
        }
        [Fact]
        public void GetTypeOfBrokerMessage_OnPowerProfilesBrokerMessageType_ReturPowerProfileTypeOfEnum()
        {
            //----------------
            var _power_profile = GeneralHelper._getPowerProfileBrokerMessage();
            var _message = JsonConvert.SerializeObject(_power_profile);

            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //-------------
            var _result = _target.Object.GetTypeOfBrokerMessage(_message);
            //------------
            Assert.Equal(broker_messages_types.power_profiles_broker_message_type, _result);
        }
        [Fact]
        public void GetTypeOfBrokerMessage_OnBrokerTaskMessageType_ReturBrokerTaskTypeOfEnum()
        {
            //----------------
            var _broker_task = GeneralHelper._getDefaultBrokerTaskMessageList().First();
            var _message = JsonConvert.SerializeObject(_broker_task);

            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //-------------
            var _result = _target.Object.GetTypeOfBrokerMessage(_message);
            //------------
            Assert.Equal(broker_messages_types.broker_task_message_type, _result);
        }
        [Fact]
        public void GetTypeOfBrokerMessage_OnEnergyRecordResponseType_ReturEnergyRecordTypeOfEnum()
        {
            //----------------
            var _energy_response = GeneralHelper._getEnergyRecordResponse();
            var _message = JsonConvert.SerializeObject(_energy_response);

            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //-------------
            var _result = _target.Object.GetTypeOfBrokerMessage(_message);
            //------------
            Assert.Equal(broker_messages_types.energy_response_message_type, _result);
        }

        [Fact]
        public void GetTypeOfBrokerMessage_OnInvalidMessageType_ReturnNull()
        {
            //----------------
            var _invalid_message = new
            {
                test1 = "1",
                test2 = 2,
                test3 = "3"
            };
            var _message = JsonConvert.SerializeObject(_invalid_message);

            var _database = new Mock<kzmp_energyContext>();
            var _conf = new Mock<IConfiguration>();

            var _target = new Mock<RabbitMessagesHandler>(_conf.Object);
            _target.Setup(t => t._CreateDatabase(_conf.Object)).Returns(_fixture.CreateContext());
            //-------------
            var _result = _target.Object.GetTypeOfBrokerMessage(_message);
            //------------
            Assert.Null(_result);
        }


    }
}
