using DataCollectionService.Components;
using DataCollectionService.Components.Models;
using DataCollectionService.Interfaces.IServices.IAppServices;
using DataCollectionService.Services.AppServices;
using DataCollectionService.Test.Helpers;
using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Interfaces.IActions;
using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Variables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using System.IO.Ports;

namespace DataCollectionService.Test.ServicesTest.AppServices
{
    public class IndicationsReaderTest
    {
        //-----------------------------------------------------------------------------------------------------------
        //GetEnergyReadingTask
        [Fact]
        public void GetEnergyReadingTask_OnInvoke_VerifyResult()
        {
            //------
            var _conf = new Mock<IConfiguration>();
            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _indic_reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);

            DateTime _start_date = new DateTime(2021, 08, 15);
            DateTime _end_date = new DateTime(2022, 01, 27);
            var _true_result = new Queue<MonthYearEnergyTask>();
            var _list = new List<MonthYearEnergyTask>()
            {
                new MonthYearEnergyTask()
                {
                    month=8,
                    year=2021
                },
                new MonthYearEnergyTask()
                {
                    month=9,
                    year=2021
                },
                new MonthYearEnergyTask()
                {
                    month=10,
                    year=2021
                },
                new MonthYearEnergyTask()
                {
                    month=11,
                    year=2021
                },
                new MonthYearEnergyTask()
                {
                    month=12,
                    year=2021
                }
            };
            foreach (var item in _list)
            {
                _true_result.Enqueue(item);
            }
            //--------------------------------------
            var _result = _indic_reader.GetEnergyReadingTask(_start_date, _end_date);
            //----------------------------------------
            Assert.Equal(_true_result.Count, _result.Count);
            foreach (var item in _true_result)
            {
                Assert.True(_result.Where(t => t.month == item.month && t.year == item.year).Any());
            }
        }
        //------------------------------------------------------------------------------------------------------------
        //GetTypeOfBrokerMessage Testing
        [Fact]
        public void GetTypeOfBrokerMessage_OnListBrokerTaskMessage_ReturnAppConstsListBrokerTaskType()
        {
            //----
            var _conf = new Mock<IConfiguration>();
            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _indic_reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);
            var _list_broker_task = GeneralHelper._getDefaultBrokerTaskMessageList();

            //---
            var _result = _indic_reader.GetTypeOfBrokerMessage(JsonConvert.SerializeObject(_list_broker_task));
            //----
            Assert.Equal(AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE, _result);
        }
        [Fact]
        public void GetTypeOfBrokerMessage_OnPortConfiguration_ReturnPortConfTypeValue()
        {
            //----
            var _conf = new Mock<IConfiguration>();
            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _indic_reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);
            var _port_conf = new PortConfiguration()
            {
                PortName = "TEST"
            };

            //-----
            var _result = _indic_reader.GetTypeOfBrokerMessage(JsonConvert.SerializeObject(_port_conf));
            //------
            Assert.Equal(AppConsts.PORT_CONFIGURATION_TYPE, _result);
        }

        [Fact]
        public void GetTypeOfBrokerMessage_OnInvalidType_ThrowResult()
        {
            //----
            var _conf = new Mock<IConfiguration>();
            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _indic_reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);

            //-----
            Assert.Throws<Exception>(() => _indic_reader.GetTypeOfBrokerMessage(JsonConvert.SerializeObject(new
            {
                Test = "test",
                Test2 = "test2",
                Test3 = "test3"
            })));
        }
        //---------------------------------------------------------------------------------------------------------------
        //GetIndications Testing

        [Fact]
        public async void GetIndications_OnInvoke_VerifyFilterBrokerTaskMessageInvoke()
        {
            //---
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns("COM1");

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new Mock<IndicationsReader>(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);
            _reader.Setup(t => t.FilterBrokerTaskMessage(_broker_message)).Returns(GeneralHelper._getFiltredBySimNumberQueueOfBrokerTaskMessageList());
            //---
            var result = await _reader.Object.GetIndications(_broker_message);
            //---
            _reader.Verify(t => t.FilterBrokerTaskMessage(_broker_message));
        }
        [Fact]
        public async void GetIndications_OnEmptyComPortName_VerifyPublishSheduleLogMessageInvoke()
        {
            //----
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault();
            var _shedule_log = new SheduleLog()
            {
                date_time = DateTime.Now,
                description = "The default COM port could not be determined",
                status = CommonVariables.ERROR_LOG_STATUS,
                shedule_id = _shedule_id ?? -1
            };

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns("");

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            _rabbit_message_gen.Setup(t => t.CreateSheduleLog(
                     _shedule_id ?? -1,
                     CommonVariables.ERROR_LOG_STATUS,
                     "The default COM port could not be determined",
                     null)).Returns(_shedule_log);

            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new Mock<IndicationsReader>(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);

            //----
            var result = await _reader.Object.GetIndications(_broker_message);

            //----
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_log));
        }
        [Fact]
        public async void GetIndications_OnEmptyComPortName_FalseResult()
        {
            //----
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns("");

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new Mock<IndicationsReader>(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);

            //----
            var result = await _reader.Object.GetIndications(_broker_message);

            //----
            Assert.False(result);
        }

        [Theory]
        [InlineData("COM1", "1234567890")]
        public async void GetIndications_AfterFilterBrokerTaskMessage_VerifyGetGSMConnectionAsyncInvoke(string _port_name, string _sim_number)
        {
            //----
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(_port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new Mock<IndicationsReader>(_conf.Object, _logger.Object, _gsm_connection, _rabbit_message_gen.Object, _session.Object);
            _reader.Setup(t => t.FilterBrokerTaskMessage(_broker_message)).Returns(GeneralHelper._getFiltredBySimNumberQueueOfBrokerTaskMessageList());

            //----
            var result = await _reader.Object.GetIndications(_broker_message);

            //----
            _session.Verify(t => t.GetGSMConnectionAsync(_port_name, _sim_number, _shedule_id));
        }

        [Theory]
        [InlineData("COM1", "1234567890")]
        public async void GetIndications_OnInvoke_VerifyCloseGSMConnectionAsync(string _port_name, string _sim_number)
        {
            //----
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _serial_port = new SerialPort();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(_port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new Mock<IndicationsReader>(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            _reader.Setup(t => t.FilterBrokerTaskMessage(_broker_message)).Returns(GeneralHelper._getFiltredBySimNumberQueueOfBrokerTaskMessageList());
            _session.Setup(t => t.GetGSMConnectionAsync(_port_name, _sim_number, _shedule_id)).ReturnsAsync(_serial_port);

            //----
            var result = await _reader.Object.GetIndications(_broker_message);

            //----
            _gsm_connection.Verify(t => t.CloseGSMConnectionAsync(ref _serial_port));
        }
        [Theory]
        [InlineData("COM1", "1234567890")]
        public async void GetIndications_OnValidGetGSMConnAsyncResult_VerifyStartIndicReadSessInvoke(string _port_name, string _sim_number)
        {
            //----
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _filtred_queue = GeneralHelper._getFiltredBySimNumberQueueOfBrokerTaskMessageList();

            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _serial_port = new SerialPort();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(_port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new Mock<IndicationsReader>(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            _reader.Setup(t => t.FilterBrokerTaskMessage(_broker_message)).Returns(_filtred_queue);
            _session.Setup(t => t.GetGSMConnectionAsync(_port_name, _sim_number, _shedule_id)).ReturnsAsync(_serial_port);

            //----
            var result = await _reader.Object.GetIndications(_broker_message);

            //----
            List<BrokerTaskMessage>? _list;
            while (_filtred_queue.TryDequeue(out _list))
            {
                _reader.Verify(t => t.StartIndicationsReadingSession(ref _serial_port, ref _list));
            }
        }


        //-------------------------------------------------------------------------------------------------------
        //StartIndicationsReadingSession
        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_OnInvoke_VerifyDetermineMeterTypeInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(GeneralHelper._getSupportedMeterType());
            }

            var _serial_port = new SerialPort();
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);

            //-----
            foreach (var _task in _broker_message)
            {
                _session.Verify(t => t.DetermineMeterType(_task.meter_type));
            }
        }
        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_OnNullResultByDetermineMeterTypeFunc_VerifyPublishSheduleLogMessageInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _shedule_logs = new SheduleLog();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var _task in _broker_message)
            {
                var _desc = $"The meter type could not be determined. [Meter type:{_task.meter_type}] [Meter adress: {_task.meter_address}] [SIM: {_task.sim_number}]";
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(_task.shedule_id ?? -1, CommonVariables.ERROR_LOG_STATUS, _desc, null)).Returns(_shedule_logs);
            }

            var _session = new Mock<IIndicationsReadingSession>();
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(() => null);
            }

            var _serial_port = new SerialPort();
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);

            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);

            //-----
            foreach (var _task in _broker_message)
            {
                _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_logs));
            }
        }

        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_OnNullResultByDetermineMeterTypeFunc_VerifyPublishFailedBrokerTaskMessageInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(() => null);
            }

            var _serial_port = new SerialPort();
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);

            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);

            //-----
            foreach (var _task in _broker_message)
            {
                _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_task));
            }
        }
        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_AfterValidResultOfDetermineMeterType_VerifyDetermineCommunicInterfInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(GeneralHelper._getSupportedMeterType());
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(GeneralHelper._getSupportedCommunicationInterface());
            }
            var _serial_port = new SerialPort();

            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);

            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);

            //-----
            foreach (var _task in _broker_message)
            {
                _session.Verify(t => t.DetermineCommunicationInterface(_task.communic_interface));
            }
        }
        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_OnNullResultOfDetermineCommunicInterfFunc_VerifyPublishFailedMessageInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(GeneralHelper._getSupportedMeterType());
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(() => null);
            }
            var _serial_port = new SerialPort();

            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);

            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);

            //-----
            foreach (var _task in _broker_message)
            {
                _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_task));
            }
        }

        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_AfterDetermineCommunicInterface_VerifyComputeStartDateForIndicationsReadingInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(GeneralHelper._getSupportedMeterType());
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(GeneralHelper._getSupportedCommunicationInterface());
            }
            var _serial_port = new SerialPort();

            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);

            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);

            //-----
            foreach (var _task in _broker_message)
            {
                _session.Verify(t => t.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001"));
            }
        }
        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_OnNullStartDate_VerifyPublishFailedMessageInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
            var _serial_port = new SerialPort();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(GeneralHelper._getSupportedMeterType());
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(GeneralHelper._getSupportedCommunicationInterface());
                _session.Setup(t => t.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001")).Returns(() => null);
                /*                _session.Setup(t => t.ReConnect(ref _serial_port, _task.sim_number ?? "", _task.shedule_id ?? -1, port_name, _reader._TIMEOUT_AFTER_FAILURE));
                */
            }
            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);
            //-----
            foreach (var _task in _broker_message)
            {
                _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_task));
            }
        }

        [Fact]
        public async void StartIndicationsReadingSession_AfterComputeStartDate_VerifyInitializeCommunicationSessionInvoke()
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _supported_communic_interface = GeneralHelper._getSupportedCommunicationInterface() ?? "";
            var _supported_meter_type = GeneralHelper._getSupportedMeterType();
            var _start_date = new DateTime(2022, 07, 01, 10, 00, 00);
            var _end_date = new DateTime(2022, 08, 01, 10, 00, 00);
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
            var _serial_port = new SerialPort();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns("COM1");

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            _session.Setup(t => t.GetCurrentDateTime()).Returns(_end_date);
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(_supported_meter_type);
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(_supported_communic_interface);
                _session.Setup(t => t.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001")).Returns(_start_date);
            }
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            //-----
            var result = await _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);
            //-----
            foreach (var _task in _broker_message)
            {
                _session.Verify(t => t.InitializeCommunicationSessionAsync(_supported_communic_interface, _supported_meter_type, _serial_port, Convert.ToInt32(_task.meter_address), _start_date, _end_date, _start_date.Month, _start_date.Year, _task.shedule_id ?? -1));
            }
        }

        [Fact]
        public void StartIndicationsReadingSession_OnNullResultOfInitCommunicSessionAsync_VerifyReConnectInvoke()
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _supported_communic_interface = GeneralHelper._getSupportedCommunicationInterface() ?? "";
            var _supported_meter_type = GeneralHelper._getSupportedMeterType();
            var _start_date = new DateTime(2022, 07, 01, 10, 00, 00);
            var _end_date = new DateTime(2022, 08, 01, 10, 00, 00);
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
            var _serial_port = new SerialPort();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns("COM1");

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();

            var _session = new Mock<IIndicationsReadingSession>();
            _session.Setup(t => t.GetCurrentDateTime()).Returns(_end_date);
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(_supported_meter_type);
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(_supported_communic_interface);
                _session.Setup(t => t.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001")).Returns(_start_date);
                _session.Setup(t => t.InitializeCommunicationSessionAsync(_supported_communic_interface, _supported_meter_type, _serial_port, Convert.ToInt32(_task.meter_address), _start_date, _end_date, _start_date.Month, _start_date.Year, _task.shedule_id ?? -1)).ReturnsAsync(() => null);
            }
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);
            //-----
            foreach (var _task in _broker_message)
            {
                _session.Verify(t => t.ReConnect(ref _serial_port, _task.sim_number ?? "", _task.shedule_id ?? -1, "COM1", _reader._TIMEOUT_AFTER_FAILURE));
            }
        }
        [Theory]
        [InlineData("COM1")]
        public void StartIndicationsReadingSession_OnNotNullResultOfInitCommunicSessionAsync_VerifyReadAndPublishInvoke(string port_name)
        {
            //---------
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            var _supported_communic_interface = GeneralHelper._getSupportedCommunicationInterface() ?? "";
            var _supported_meter_type = GeneralHelper._getSupportedMeterType();
            DateTime? _start_date = new DateTime(2022, 07, 01, 10, 00, 00);
            var _month = _start_date?.Month ?? -1;
            var _year = _start_date?.Year ?? -1;
            var _end_date = new DateTime(2022, 08, 01, 10, 00, 00);
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
            var _serial_port = new SerialPort();

            var _conf = new Mock<IConfiguration>();
            _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

            var _logger = new Mock<ILogger<IndicationsReader>>();
            var _gsm_connection = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _common_indic_reader = new Mock<ICommonIndicationsReader>();

            var _session = new Mock<IIndicationsReadingSession>();
            _session.Setup(t => t.GetCurrentDateTime()).Returns(_end_date);
            foreach (var _task in _broker_message)
            {
                _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(_supported_meter_type);
                _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(_supported_communic_interface);
                _session.Setup(t => t.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001")).Returns(_start_date);
                _session.Setup(t => t.InitializeCommunicationSessionAsync(_supported_communic_interface, _supported_meter_type, _serial_port, Convert.ToInt32(_task.meter_address), _start_date ?? _end_date, _end_date, _month, _year, _task.shedule_id ?? -1)).ReturnsAsync(_common_indic_reader.Object);

                _session.Setup(t => t.ReadAndPublishPowerProfileIndications(_common_indic_reader.Object, _rabbit_message_gen.Object, _task.shedule_id ?? -1, _task.meter_id ?? -1, ref _start_date, ref _end_date)).Returns(true);
            }
            var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
            //-----
            var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);
            //-----
            foreach (var _task in _broker_message)
            {
                _session.Verify(t => t.ReadAndPublishPowerProfileIndications(_common_indic_reader.Object, _rabbit_message_gen.Object, _task.shedule_id ?? -1, _task.meter_id ?? -1, ref _start_date, ref _end_date));
            }
        }
        /*        [Theory]
                [InlineData("COM1")]
                public void StartIndicationsReadingSession_AfterPowerProfileReading_VerifyReadAndPublishEnergyIndicInvoke(string port_name)
                {
                    //---------
                    var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
                    var _supported_communic_interface = GeneralHelper._getSupportedCommunicationInterface() ?? "";
                    var _supported_meter_type = GeneralHelper._getSupportedMeterType();
                    DateTime? _start_date = new DateTime(2022, 07, 01, 10, 00, 00);
                    var _month = _start_date?.Month ?? -1;
                    var _year = _start_date?.Year ?? -1;
                    var _end_date = new DateTime(2022, 08, 01, 10, 00, 00);
                    int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
                    var _serial_port = new SerialPort();

                    var _conf = new Mock<IConfiguration>();
                    _conf.Setup(t => t["DEFAULT_COM_PORT"]).Returns(port_name);

                    var _logger = new Mock<ILogger<IndicationsReader>>();
                    var _gsm_connection = new Mock<IGSMConnection>();
                    var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
                    var _common_indic_reader = new Mock<ICommonIndicationsReader>();

                    var _session = new Mock<IIndicationsReadingSession>();
                    _session.Setup(t => t.GetCurrentDateTime()).Returns(_end_date);
                    foreach (var _task in _broker_message)
                    {
                        _session.Setup(t => t.DetermineMeterType(_task.meter_type)).Returns(_supported_meter_type);
                        _session.Setup(t => t.DetermineCommunicationInterface(_task.communic_interface)).Returns(_supported_communic_interface);
                        _session.Setup(t => t.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001")).Returns(_start_date);
                        _session.Setup(t => t.InitializeCommunicationSessionAsync(_supported_communic_interface, _supported_meter_type, _serial_port, Convert.ToInt32(_task.meter_address), _start_date ?? _end_date, _end_date, _month, _year, _task.shedule_id ?? -1)).ReturnsAsync(_common_indic_reader.Object);

                        _session.Setup(t => t.ReadAndPublishPowerProfileIndications(_common_indic_reader.Object, _rabbit_message_gen.Object, _task.shedule_id ?? -1, _task.meter_id ?? -1, ref _start_date, ref _end_date)).Returns(true);
                    }
                    var _reader = new IndicationsReader(_conf.Object, _logger.Object, _gsm_connection.Object, _rabbit_message_gen.Object, _session.Object);
                    //-----
                    var result = _reader.StartIndicationsReadingSession(ref _serial_port, ref _broker_message);
                    //-----
                    foreach (var _task in _broker_message)
                    {
                        _session.Verify(t => t.ReadAndPublishEnergyIndicationsAsync(_common_indic_reader.Object, _rabbit_message_gen.Object, _task.shedule_id ?? -1, _task.meter_id ?? -1, 1, 2001));
                    }
                }
        */
    }
}
