using DataCollectionService.Interfaces.IServices.IAppServices;
using DataCollectionService.Services.AppServices;
using DataCollectionService.Test.Helpers;
using KzmpEnergyIndicationsLibrary.Devices.GatewayGSM;
using KzmpEnergyIndicationsLibrary.Devices.ModemGSM;
using KzmpEnergyIndicationsLibrary.Interfaces.IActions;
using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Test.ServicesTest.AppServices
{
    public class IndicationsReadingSessionTest
    {
        //---------------------------------------------------
        //ReadAndPublishEnergyIndicationsAsync
        [Theory]
        [InlineData(1, 2, 1, 2001)]
        public async void ReadAndPublishEnergyIndicationsAsync_OnInvoke_VerifyGetEnergyRecordAsyncInvoke(int shedule_id, int meter_id, int month, int year)
        {
            //-----
            var _energy_response = GeneralHelper._getEnergyRecordResponse();
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetEnergyRecordAsync(month, year)).ReturnsAsync(_energy_response);
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new IndicationsReadingSession(_gsm_conn, _rabbit_message_gen, _logger);
            //------
            var _result = await _target.ReadAndPublishEnergyIndicationsAsync(_indic_reader.Object, _rabbit_message_gen, shedule_id, meter_id, month, year);
            //------
            _indic_reader.Verify(t => t.GetEnergyRecordAsync(month, year));
        }
        [Theory]
        [InlineData(1, 2, 1, 2001)]
        public async void ReadAndPublishEnergyIndicationsAsync_OnGetEnergyRecordAsyncException_FalseResult(int shedule_id, int meter_id, int month, int year)
        {
            //-----
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetEnergyRecordAsync(month, year)).ThrowsAsync(new Exception());

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new IndicationsReadingSession(_gsm_conn, _rabbit_message_gen, _logger);
            //------
            await Assert.ThrowsAsync<Exception>(() => _target.ReadAndPublishEnergyIndicationsAsync(_indic_reader.Object, _rabbit_message_gen, shedule_id, meter_id, month, year));
        }
        [Theory]
        [InlineData(1, 2, 1, 2001)]
        public async void ReadAndPublishEnergyIndicationsAsync_OnGetEnergyRecordAsyncException_VerifyPublishSheduleLogInvoke(int shedule_id, int meter_id, int month, int year)
        {
            //-----
            var _log = new SheduleLog()
            {
                date_time = DateTime.Now,
                description = "",
                shedule_id = shedule_id,
                status = "error"
            };
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, CommonVariables.ERROR_LOG_STATUS, "Exception of type 'System.Exception' was thrown.", null)).Returns(_log);
            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetEnergyRecordAsync(month, year)).ThrowsAsync(new Exception());

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen.Object, _logger);
            //------
            var _result = await _target.Object.ReadAndPublishEnergyIndicationsAsync(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, meter_id, month, year);
            //------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_log));
        }
        [Theory]
        [InlineData(1, 2, 1, 2001)]
        public async void ReadAndPublishEnergyIndicationsAsync_AfterGetEnergyRecordAsync_VerifyPublishSheduleLogInvoke(int shedule_id, int meter_id, int month, int year)
        {
            //-----
            var _energy_response = GeneralHelper._getEnergyRecordResponse();
            var _logs_count = _energy_response.Logs.Count;
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var _log in _energy_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, _log.Status ?? CommonVariables.WARNING_LOG_STATUS, _log.Description ?? "", _log.Date)).Returns(_shedule_log);
            }

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetEnergyRecordAsync(month, year)).ReturnsAsync(_energy_response);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new IndicationsReadingSession(_gsm_conn, _rabbit_message_gen.Object, _logger);
            //------
            var _result = await _target.ReadAndPublishEnergyIndicationsAsync(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, meter_id, month, year);
            //------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_log), Times.AtLeast(_logs_count));
        }
        [Theory]
        [InlineData(1, 2, 1, 2001)]
        public async void ReadAndPublishEnergyIndicationsAsync_OnInvoke_VerifyPublishResponseInvoke(int shedule_id, int meter_id, int month, int year)
        {
            //-----
            var _energy_response = GeneralHelper._getEnergyRecordResponse();
            var _logs_count = _energy_response.Logs.Count;
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var _log in _energy_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, _log.Status ?? CommonVariables.WARNING_LOG_STATUS, _log.Description ?? "", _log.Date)).Returns(_shedule_log);
            }

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetEnergyRecordAsync(month, year)).ReturnsAsync(_energy_response);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new IndicationsReadingSession(_gsm_conn, _rabbit_message_gen.Object, _logger);
            //------
            var _result = await _target.ReadAndPublishEnergyIndicationsAsync(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, meter_id, month, year);
            //------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_energy_response), Times.Once);
        }
        //---------------------------------------------------
        //Func: DetermineMeterType
        [Theory]
        [InlineData("Меркурий 230")]
        [InlineData("меРкУрИй 234")]
        public void DetermineMeterType_OnSupportedMeterTypeParam_IMeterTypeResult(string meter_type)
        {
            //-----
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            _target.Setup(t => t.GetValidMeterTypes()).Returns(GeneralHelper._getDefaultMetersTypes());
            //-----
            var result = _target.Object.DetermineMeterType(meter_type);
            //----
            Assert.IsAssignableFrom<IMeterType>(result);
        }

        [Theory]
        [InlineData("Нептун 230")]
        [InlineData("Меркурий 033")]
        public void DetermineMeterType_OnUnSupportedMeterTypeParam_NullResult(string meter_type)
        {
            //-----
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            _target.Setup(t => t.GetValidMeterTypes()).Returns(GeneralHelper._getDefaultMetersTypes());
            //-----
            var result = _target.Object.DetermineMeterType(meter_type);
            //----
            Assert.Null(result);
        }
        //------------------------------------------------------------
        //Func: DetermineCommunicationInterface
        [Theory]
        [InlineData("gsm")]
        public void DetermineCommunicationInterface_OnInvoke_VerifyGetValidCommunicInterfacesInvoke(string communic_interface)
        {
            //-----------
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            _target.Setup(t => t.GetValidCommunicationInterfaces()).Returns(GeneralHelper._getDefaultCommunicationInterfaces());
            //------------
            var result = _target.Object.DetermineCommunicationInterface(communic_interface);
            //------------
            _target.Verify(t => t.GetValidCommunicationInterfaces());
        }
        [Theory]
        [InlineData("gsm")]
        [InlineData("GsM- шл Юз")]
        public void DetermineCommunicationInterface_OnSupportedCommunicInterface_StringTypeResult(string communic_interface)
        {
            //-----------
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            _target.Setup(t => t.GetValidCommunicationInterfaces()).Returns(GeneralHelper._getDefaultCommunicationInterfaces());
            //------------
            var result = _target.Object.DetermineCommunicationInterface(communic_interface);
            //------------
            Assert.IsType<String>(result);
        }

        [Theory]
        [InlineData("Car")]
        [InlineData("Computer ")]
        public void DetermineCommunicationInterface_OnUnSupportedCommunicInterface_NullResult(string communic_interface)
        {
            //-----------
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            _target.Setup(t => t.GetValidCommunicationInterfaces()).Returns(GeneralHelper._getDefaultCommunicationInterfaces());
            //------------
            var result = _target.Object.DetermineCommunicationInterface(communic_interface);
            //------------
            Assert.Null(result);
        }
        //---------------------------------------------------------------------------------------
        //Func: ComputeStartDateForIndicationsReading
        [Theory]
        [InlineData("invalid date", "01.01.2001")]
        [InlineData("01.01.2001", "invalid date")]
        [InlineData("invalid", "invalid")]
        public void ComputeStartDateForIndicationsReading_OnInvalidParams_NullResult(string start_date, string last_reading_date)
        {
            //------
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            //----------
            var _result = _target.Object.ComputeStartDateForIndicationsReading(start_date, last_reading_date);
            //---------
            Assert.Null(_result);
        }
        [Theory]
        [InlineData("01.01.2001", "20.01.2001")]
        [InlineData("01.01.2001 09:30:00", "01.01.2001 10:00:00")]
        public void ComputeStartDateForIndicationsReading_OnStartDateEarlierLastReadingDate_LastReadingDateTimeResult(string start_date, string last_reading_date)
        {
            //------
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            //----------
            var _result = _target.Object.ComputeStartDateForIndicationsReading(start_date, last_reading_date);
            //---------
            Assert.Equal(DateTime.Parse(last_reading_date), _result);
        }
        [Theory]
        [InlineData("20.01.2001", "01.01.2001")]
        [InlineData("01.01.2001 10:00:00", "01.01.2001 09:30")]
        public void ComputeStartDateForIndicationsReading_OnStartDateLaterLastReadingDate_LastReadingDateTimeResult(string start_date, string last_reading_date)
        {
            //------
            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            //----------
            var _result = _target.Object.ComputeStartDateForIndicationsReading(start_date, last_reading_date);
            //---------
            Assert.Equal(DateTime.Parse(start_date), _result);
        }
        //------------------------------------------------------------------------
        //Func: InitializeCommunicationSession
        [Fact]
        public async void InitializeCommunicationSession_OnNotValidSerialPort_NullResult()
        {
            //----------------
            var _communic_interface = GeneralHelper._getSupportedCommunicationInterface();
            var _meter_type = GeneralHelper._getSupportedMeterType();
            var _address = 1;
            var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = DateTime.Now;
            var _serial_port = new SerialPort();
            var _gsm_conn = new Mock<IGSMConnection>();
            _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(false);

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new IndicationsReadingSession(_gsm_conn.Object, _rabbit_message_gen, _logger);
            //---------------
            var _result = await _target.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
            //---------------
            Assert.Null(_result);
        }

        /*        [Theory]
                [InlineData("GSM")]
                public async void InitializeCommunicationSession_OnGSMCommunicInterface_ReturnMercury230_234_ModemGSMTypeResult(string _communic_interface)
                {
                    //----------------
                    var _meter_type = GeneralHelper._getSupportedMeterType();
                    var _address = 1;
                    var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
                    var _end_date = DateTime.Now;
                    var _serial_port = new SerialPort();
                    var _gsm_conn = new Mock<IGSMConnection>();
                    _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(true);

                    var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
                    var _target = new IndicationsReadingSession(_gsm_conn.Object, _rabbit_message_gen);
                    //---------------
                    var _result = await _target.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
                    //----------------
                    Assert.IsType<Mercury230_234_ModemGSM>(_result);
                }
                [Theory]
                [InlineData("GSM-шлюз")]
                public async void InitializeCommunicationSession_OnGatewayCommunicInterface_ReturnMercury230_234_GatewayGSMTypeResult(string _communic_interface)
                {
                    //----------------
                    var _meter_type = GeneralHelper._getSupportedMeterType();
                    var _address = 1;
                    var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
                    var _end_date = DateTime.Now;
                    var _serial_port = new SerialPort();
                    var _session_response = GeneralHelper._getValidSessionInitResponse();

                    var _gsm_conn = new Mock<IGSMConnection>();
                    _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(true);

                    var _common_indic_reader = new Mock<ICommonIndicationsReader>();
                    _common_indic_reader.Setup(t => t.SessionInitializationAsync()).ReturnsAsync(_session_response);

                    var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;

                    var _target = new Mock<IndicationsReadingSession>(_gsm_conn.Object, _rabbit_message_gen);
                    _target.Setup(t => t.CreateCommonIndicReader(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year)).Returns(_common_indic_reader.Object);
                    //---------------
                    var _result = await _target.Object.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
                    //----------------
                    Assert.IsType<Mercury230_234_GatewayGSM>(_result);
                }*/
        [Theory]
        [InlineData("invalid communic interface")]
        public async void InitializeCommunicationSession_OnInvalidCommunicInterface_ReturnNullResult(string _communic_interface)
        {
            //----------------
            var _meter_type = GeneralHelper._getSupportedMeterType();
            var _address = 1;
            var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = DateTime.Now;
            var _serial_port = new SerialPort();
            var _gsm_conn = new Mock<IGSMConnection>();
            _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(true);

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new IndicationsReadingSession(_gsm_conn.Object, _rabbit_message_gen, _logger);
            //---------------
            var _result = await _target.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
            //----------------
            Assert.Null(_result);
        }

        [Theory]
        [InlineData("GSM-шлюз")]
        [InlineData("GSM")]
        public async void InitializeCommunicationSession_OnValidCommunicInterface_VerifySessionInitializationAsyncInvoke(string _communic_interface)
        {
            //----------------
            var _meter_type = GeneralHelper._getSupportedMeterType();
            var _address = 1;
            var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = DateTime.Now;
            var _serial_port = new SerialPort();
            var _gsm_conn = new Mock<IGSMConnection>();
            _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(true);

            var _common_indic_reader = new Mock<ICommonIndicationsReader>();
            _common_indic_reader.Setup(t => t.SessionInitializationAsync()).ReturnsAsync(GeneralHelper._getValidSessionInitResponse());

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn.Object, _rabbit_message_gen, _logger);
            _target.Setup(t => t.CreateCommonIndicReader(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year)).Returns(_common_indic_reader.Object);
            //---------------
            var _result = await _target.Object.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
            //----------------
            _common_indic_reader.Verify(t => t.SessionInitializationAsync());
        }

        [Theory]
        [InlineData("GSM")]
        public async void InitializeCommunicationSession_OnNotNullSessionInitLogs_VerifyPublishSheduleLogMessageInvoke(string _communic_interface)
        {
            //----------------
            var _meter_type = GeneralHelper._getSupportedMeterType();
            var _address = 1;
            var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = DateTime.Now;
            var _serial_port = new SerialPort();
            var _session_response = GeneralHelper._getValidSessionInitResponse();
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();

            var _gsm_conn = new Mock<IGSMConnection>();
            _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(true);

            var _common_indic_reader = new Mock<ICommonIndicationsReader>();
            _common_indic_reader.Setup(t => t.SessionInitializationAsync()).ReturnsAsync(_session_response);

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var _log in _session_response.LogsQueue)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(1, _log.Status, _log.Description, null)).Returns(_shedule_log);
            }

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn.Object, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.CreateCommonIndicReader(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year)).Returns(_common_indic_reader.Object);
            //---------------
            var _result = await _target.Object.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
            //----------------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_log));
        }
        [Theory]
        [InlineData("GSM")]
        public async void InitializeCommunicationSession_OnNotNullSessionInitException_NullResult(string _communic_interface)
        {
            //----------------
            var _meter_type = GeneralHelper._getSupportedMeterType();
            var _address = 1;
            var _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = DateTime.Now;
            var _serial_port = new SerialPort();
            var _session_response = GeneralHelper._getValidSessionInitResponse();
            _session_response.ExceptionMessage = "some exception occured";

            var _shedule_log = GeneralHelper._getDefaultSheduleLog();

            var _gsm_conn = new Mock<IGSMConnection>();
            _gsm_conn.Setup(t => t.CheckSerialPortOpenAndCDHolding(_serial_port)).Returns(true);

            var _common_indic_reader = new Mock<ICommonIndicationsReader>();
            _common_indic_reader.Setup(t => t.SessionInitializationAsync()).ReturnsAsync(_session_response);

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var _log in _session_response.LogsQueue)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(1, _log.Status, _log.Description, null)).Returns(_shedule_log);
            }

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn.Object, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.CreateCommonIndicReader(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year)).Returns(_common_indic_reader.Object);
            //---------------
            var _result = await _target.Object.InitializeCommunicationSessionAsync(_communic_interface, _meter_type, _serial_port, _address, _start_date, _end_date, _start_date.Month, _start_date.Year, 1);
            //----------------
            Assert.Null(_result);
        }
        //-----------------------------------------------------------------------------------------------------------------
        //GetGSMConnectionAsync Testing

        [Theory]
        [InlineData("COM1", "89000000000", 1)]
        public async void GetGsmConnectionAsync_OnInvoke_VerifyCreateGSMConnectionAsyncInvoke(string _port_name, string sim_number, int shedule_id)
        {
            //-----
            var _gsm_conn = new Mock<IGSMConnection>();
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _session = new Mock<IndicationsReadingSession>(_gsm_conn.Object, _rabbit_message_gen.Object, _logger);
            //-----
            var result = await _session.Object.GetGSMConnectionAsync(_port_name, sim_number, shedule_id);

            //-----
            _gsm_conn.Verify(t => t.CreateGSMConnectionAsync(sim_number, "71,0,1", 500, 20000));
        }
        [Theory]
        [InlineData("COM1", "89000000000", 1)]
        public async void GetGsmConnectionAsync_OnCreateGSMConnectionAsyncException_VerifyPublishLogMessageInvoke(string _port_name, string sim_number, int shedule_id)
        {
            //-----
            var _broker_message = GeneralHelper._getDefaultBrokerTaskMessageList();
            int _shedule_id = _broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
            var _shedule_log = new SheduleLog()
            {
                date_time = DateTime.Now,
                description = "Failed to create connection. Retrying...",
                status = CommonVariables.WARNING_LOG_STATUS,
                shedule_id = _shedule_id
            };


            var _gsm_connection = new Mock<IGSMConnection>();
            _gsm_connection.Setup(t => t.CreateGSMConnectionAsync(sim_number, "71,0,1", 500, 20000)).ThrowsAsync(new Exception());

            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            _rabbit_message_gen.Setup(t => t.CreateSheduleLog(_shedule_id, CommonVariables.WARNING_LOG_STATUS, "Failed to create connection. Retrying...", null)).Returns(_shedule_log);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _session = new Mock<IndicationsReadingSession>(_gsm_connection.Object, _rabbit_message_gen.Object, _logger);

            //-----
            var result = await _session.Object.GetGSMConnectionAsync(_port_name, sim_number, _shedule_id);

            //-----
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_log));
        }
        //--------------------------------------------------------------------------------------------------------------------
        //Func:ReadAndPublishPowerProfileIndications
        [Fact]
        public void ReadAndPublishPowerProfileIndications_OnInvoke_VerifyGetPowerProfileReadingRequestsCountInvoke()
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;
            var _indic_reader = new Mock<ICommonIndicationsReader>().Object;

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader, _rabbit_message_gen, 1, 1, ref _start_date, ref _end_date);
            //------------
            _target.Verify(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date));
        }

        [Fact]
        public void ReadAndPublishPowerProfileIndications_OnInvoke_VerifyGetPowerProfileRecordAsyncInvoke()
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>().Object;

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetPowerProfileRecordAsync()).ReturnsAsync(GeneralHelper._getValidPowerProfileRecordResponse());

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen, _logger);
            _target.Setup(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date)).Returns(30);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader.Object, _rabbit_message_gen, 1, 1, ref _start_date, ref _end_date);
            //------------
            _indic_reader.Verify(t => t.GetPowerProfileRecordAsync(), Times.Exactly(30));
        }
        [Theory]
        [InlineData(1, 30)]
        public void ReadAndPublishPowerProfileIndications_OnInvoke_VerifyPublishSheduleLogInvoke(int shedule_id, int requests_count)
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);
            var _power_profile_record_response = GeneralHelper._getValidPowerProfileRecordResponse();
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _publish_shedule_log_invoke_count = _power_profile_record_response.Logs.Count;

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var log in _power_profile_record_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, log.Status ?? "", log.Description ?? "", log.Date)).Returns(_shedule_log);
            }

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetPowerProfileRecordAsync()).ReturnsAsync(_power_profile_record_response);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date)).Returns(requests_count);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, 1, ref _start_date, ref _end_date);
            //------------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_log), Times.AtLeast(_publish_shedule_log_invoke_count));
        }
        [Theory]
        [InlineData(1, 30)]
        public void ReadAndPublishPowerProfileIndications_OnNotNullExceptionFieldInResponse_FalseResultReturn(int shedule_id, int requests_count)
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);

            var _power_profile_record_response = GeneralHelper._getValidPowerProfileRecordResponse();
            _power_profile_record_response.Exception = "SOME EXCEPTION";

            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _publish_shedule_log_invoke_count = _power_profile_record_response.Logs.Count;

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var log in _power_profile_record_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, log.Status ?? "", log.Description ?? "", log.Date)).Returns(_shedule_log);
            }

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetPowerProfileRecordAsync()).ReturnsAsync(_power_profile_record_response);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date)).Returns(requests_count);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, 1, ref _start_date, ref _end_date);
            //------------
            Assert.False(_result);
        }
        [Theory]
        [InlineData(1, 30)]
        public void ReadAndPublishPowerProfileIndications_OnNotNullExceptionFieldInResponse_VerifyPublishSheduleLogMessage(int shedule_id, int requests_count)
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);

            var _power_profile_record_response = GeneralHelper._getValidPowerProfileRecordResponse();
            _power_profile_record_response.Exception = "SOME EXCEPTION";

            var _shedule_log = GeneralHelper._getDefaultSheduleLog();

            var _publish_shedule_log_invoke_count = _power_profile_record_response.Logs.Count;

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var log in _power_profile_record_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, log.Status ?? "", log.Description ?? "", log.Date)).Returns(_shedule_log);
            }
            _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, CommonVariables.ERROR_LOG_STATUS, _power_profile_record_response.Exception, null)).Returns(_shedule_log);

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetPowerProfileRecordAsync()).ReturnsAsync(_power_profile_record_response);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date)).Returns(requests_count);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, 1, ref _start_date, ref _end_date);
            //------------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_shedule_log));
        }
        [Theory]
        [InlineData(1, 1, 30)]
        public void ReadAndPublishPowerProfileIndications_OnNullExceptionFieldInResponse_VerifyPublishPowerProfilesInvoke(int shedule_id, int meter_id, int requests_count)
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);
            var _power_profile_record_response = GeneralHelper._getValidPowerProfileRecordResponse();
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _publish_shedule_log_invoke_count = _power_profile_record_response.Logs.Count;
            var _message = GeneralHelper._getPowerProfileBrokerMessage();

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var log in _power_profile_record_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, log.Status ?? "", log.Description ?? "", log.Date)).Returns(_shedule_log);
            }

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetPowerProfileRecordAsync()).ReturnsAsync(_power_profile_record_response);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date)).Returns(requests_count);
            _target.Setup(t => t.CreatePowerProfilesBrokerMessage(meter_id, shedule_id, _power_profile_record_response.Records)).Returns(_message);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, meter_id, ref _start_date, ref _end_date);
            //------------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_message));
        }

        [Theory]
        [InlineData(1, 1, 30)]
        public void ReadAndPublishPowerProfileIndications_OnExceptionOnGetPowerProfileRecord_VerifyPublishSheduleLogMessage(int shedule_id, int meter_id, int requests_count)
        {
            //-------------
            DateTime? _start_date = new DateTime(2001, 01, 01, 00, 00, 00);
            var _end_date = new DateTime(2001, 02, 01, 00, 00, 00);
            var _power_profile_record_response = GeneralHelper._getValidPowerProfileRecordResponse();
            var _shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _ex_shedule_log = GeneralHelper._getDefaultSheduleLog();
            var _publish_shedule_log_invoke_count = _power_profile_record_response.Logs.Count;
            var _message = GeneralHelper._getPowerProfileBrokerMessage();
            var _exception = new Exception();

            var _gsm_conn = new Mock<IGSMConnection>().Object;
            var _rabbit_message_gen = new Mock<IRabbitMessageGen>();
            foreach (var log in _power_profile_record_response.Logs)
            {
                _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, log.Status ?? "", log.Description ?? "", log.Date)).Returns(_shedule_log);
            }
            _rabbit_message_gen.Setup(t => t.CreateSheduleLog(shedule_id, CommonVariables.ERROR_LOG_STATUS, $"One or more errors occurred. ({_exception.Message})", null)).Returns(_ex_shedule_log);

            var _indic_reader = new Mock<ICommonIndicationsReader>();
            _indic_reader.Setup(t => t.GetPowerProfileRecordAsync()).ThrowsAsync(_exception);

            var _logger = new Mock<ILogger<IndicationsReadingSession>>().Object;
            var _target = new Mock<IndicationsReadingSession>(_gsm_conn, _rabbit_message_gen.Object, _logger);
            _target.Setup(t => t.GetPowerProfileReadingRequestsCount(_start_date ?? _end_date, _end_date)).Returns(requests_count);
            _target.Setup(t => t.CreatePowerProfilesBrokerMessage(meter_id, shedule_id, _power_profile_record_response.Records)).Returns(_message);
            //-----------
            var _result = _target.Object.ReadAndPublishPowerProfileIndications(_indic_reader.Object, _rabbit_message_gen.Object, shedule_id, meter_id, ref _start_date, ref _end_date);
            //------------
            _rabbit_message_gen.Verify(t => t.PublishMessageToRabbit(_ex_shedule_log));
        }
    }
}
