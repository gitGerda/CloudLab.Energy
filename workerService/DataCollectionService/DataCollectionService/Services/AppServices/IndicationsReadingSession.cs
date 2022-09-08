using DataCollectionService.Interfaces.IServices.IAppServices;
using KzmpEnergyIndicationsLibrary.Actions.Communicating;
using KzmpEnergyIndicationsLibrary.Devices.GatewayGSM;
using KzmpEnergyIndicationsLibrary.Devices.ModemGSM;
using KzmpEnergyIndicationsLibrary.Interfaces.IActions;
using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Services.AppServices
{
    public class IndicationsReadingSession : IIndicationsReadingSession
    {
        private IGSMConnection _gsm_connection;
        private IRabbitMessageGen _rabbit_message_gen;
        private ILogger<IndicationsReadingSession> _logger;
        public IndicationsReadingSession(IGSMConnection gsm_connection, IRabbitMessageGen rabbit_message_gen, ILogger<IndicationsReadingSession> logger)
        {
            _gsm_connection = gsm_connection;
            _rabbit_message_gen = rabbit_message_gen;
            _logger = logger;
        }
        public void ReConnect(ref SerialPort serial_port, string sim_number, int shedule_id, string com_port_name, int timeout)
        {
            _logger.LogInformation("CLOSE GSM CONNECTION");
            _gsm_connection.CloseGSMConnectionAsync(ref serial_port).Wait();

            _logger.LogInformation($"TIMEOUT BEFORE CREATING NEW CONNECTION...[{timeout}]");
            Task.Delay(timeout).Wait();
            _logger.LogInformation("TIMEOUT END");
            serial_port = GetGSMConnectionAsync(port_name: com_port_name,
                sim_number: sim_number, shedule_id: shedule_id).Result ?? serial_port;
        }

        public async Task<bool> ReadAndPublishEnergyIndicationsAsync(ICommonIndicationsReader indic_reader, IRabbitMessageGen rabbit_message_gen, int shedule_id, int meter_id, int month, int year)
        {
            try
            {
                //Считывание показаний
                var _response = await indic_reader.GetEnergyRecordAsync(month, year);
                //Отправка логов в брокер
                while (_response.Logs.Count != 0)
                {
                    var _log = _response.Logs.Dequeue();
                    var _shedule_log = rabbit_message_gen.CreateSheduleLog(
                        shedule_id: shedule_id,
                        status: _log.Status ?? CommonVariables.WARNING_LOG_STATUS,
                        description: _log.Description ?? "",
                        date: _log.Date);
                    _logger.LogInformation(_log.Description ?? "");
                    _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                }
                //Отправка показаний энергий в брокер
                _response.meter_id = meter_id;
                _response.shedule_id = shedule_id;
                _rabbit_message_gen.PublishMessageToRabbit(_response);
                _logger.LogInformation("ENERGY RESPONSE: " + JsonConvert.SerializeObject(_response, Formatting.Indented));

                return true;
            }
            catch (Exception ex)
            {
                //Отправка логов в брокер
                var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                    description: ex.Message,
                    status: CommonVariables.ERROR_LOG_STATUS,
                    shedule_id: shedule_id,
                    date: null);
                _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                _logger.LogError(ex.Message);
                _GenerateException(ex.Message);
                return false;
            }
        }
        internal virtual void _GenerateException(string exception_message)
        {
            throw new Exception(exception_message);
        }
        public bool ReadAndPublishPowerProfileIndications(ICommonIndicationsReader indic_reader, IRabbitMessageGen rabbit_message_gen, int shedule_id, int meter_id, ref DateTime? start_date, ref DateTime end_date)
        {
            try
            {
                //Подчсёт количества часов между стартовой и конечной датами считывания
                var _reading_requests_count = GetPowerProfileReadingRequestsCount(start_date ?? end_date, end_date);
                //Запрос считывания показаний профиля мощности и отправка показаний в брокер
                for (Int32 i = 0; i < _reading_requests_count; i++)
                {
                    PowerProfileRecordResponse _response = indic_reader.GetPowerProfileRecordAsync().Result;
                    //Отправка логов в брокер
                    while (_response.Logs.Count != 0)
                    {
                        var _log = _response.Logs.Dequeue();
                        var _shedule_log = rabbit_message_gen.CreateSheduleLog(shedule_id: shedule_id,
                            status: _log.Status ?? "",
                            description: _log.Description ?? "",
                            date: _log.Date);
                        _logger.LogInformation(_log.Description ?? "");
                        rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                    }
                    //Проверка поля Exception объекта ответа
                    if (!String.IsNullOrEmpty(_response.Exception))
                    {
                        var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                            description: _response.Exception,
                            status: CommonVariables.ERROR_LOG_STATUS,
                            shedule_id: shedule_id,
                            date: null);
                        _logger.LogInformation(_response.Exception);
                        _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                        return false;
                    }
                    //Если поле Exception пусто - отправка показаний профиля мощности в брокер
                    var _power_profiles_message = CreatePowerProfilesBrokerMessage(meter_id, shedule_id, _response.Records);
                    rabbit_message_gen.PublishMessageToRabbit(_power_profiles_message);
                    //start_date + 1 час при успешном считывании и отправке в брокер
                    start_date = start_date?.AddHours(1);
                    _logger.LogInformation($"Start date: {start_date} [OK]");
                    _logger.LogInformation("POWER_PROFILE_MESSAGE: " + JsonConvert.SerializeObject(_power_profiles_message, Formatting.Indented));
                    _logger.LogInformation("---------------------------------------------------------------------------------");
                }

                return true;
            }
            catch (Exception ex)
            {
                // Отправка логов об ошибке
                var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                    description: ex.Message,
                    status: CommonVariables.ERROR_LOG_STATUS,
                    shedule_id: shedule_id,
                    date: null);
                _logger.LogInformation(ex.Message);
                _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                return false;
            }
        }
        internal virtual PowerProfilesBrokerMessage CreatePowerProfilesBrokerMessage(int meter_id, int shedule_id, Queue<PowerProfileRecord> records)
        {
            return new PowerProfilesBrokerMessage()
            {
                meter_id = meter_id,
                shedule_id = shedule_id,
                Records = records
            };
        }
        internal virtual int GetPowerProfileReadingRequestsCount(DateTime start_date, DateTime end_date)
        {
            return CommunicCommon.ComputeHalfHoursCount(start_date, end_date) / 2;
        }
        public async Task<SerialPort?> GetGSMConnectionAsync(string port_name, string sim_number, int shedule_id)
        {
            for (Int32 i = 0; i < CommonVariables.REPEAT_CONNECTION_ATTEMPTS_COUNT; i++)
            {
                try
                {
                    _logger.LogInformation("GET GSM CONNECTION....");
                    var _communic_port = await _gsm_connection.CreateGSMConnectionAsync(simNumber: sim_number, readTimeout: 20000);
                    _logger.LogInformation("GET GSM CONNECTION....OK");
                    return _communic_port;
                }
                catch (Exception ex)
                {
                    var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                        description: "Failed to create connection. Retrying...",
                        status: CommonVariables.WARNING_LOG_STATUS,
                        shedule_id: shedule_id,
                        date: null);
                    _logger.LogWarning($"FAIL TO GET CONNECTION. [{ex.Message}]");
                    _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                }
                _logger.LogWarning($"TIMEOUT: {CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC}");
                await SetDelay(CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC);
            }
            return null;
        }
        internal virtual Task SetDelay(int milisec)
        {
            Task.Delay(milisec).Wait();
            return Task.CompletedTask;
        }
        public async Task<ICommonIndicationsReader?> InitializeCommunicationSessionAsync(string communication_interface, IMeterType meterType, SerialPort serialPort, int address, DateTime startDate, DateTime endDate, int energyMonth, int energyYear, int shedule_id)
        {
            //Проверка открыт ли SerialPort и установлено ли соединение (CDHolding)
            if (!_gsm_connection.CheckSerialPortOpenAndCDHolding(serialPort)) //not async
                return null;
            //Создание объекта считывателя
            ICommonIndicationsReader? _common_indic_reader = CreateCommonIndicReader(communication_interface, meterType, serialPort, address, startDate, endDate, energyMonth, energyYear);
            if (_common_indic_reader == null)
                return null;
            //Инициализация сеанса со счётчиком
            SessionInitializationResponse _session_init_result = await _common_indic_reader.SessionInitializationAsync();
            //Обработка результата инициализации: отправка логов в брокер, если поле сообщения
            //исключения не пустое - возвращается null
            if (_session_init_result.LogsQueue != null)
            {
                Logs _logs;
                while (_session_init_result.LogsQueue.TryDequeue(out _logs))
                {
                    var _shedule_log = _rabbit_message_gen.CreateSheduleLog(shedule_id: shedule_id, status: _logs.Status ?? "", description: _logs.Description ?? "", null);
                    _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                    _logger.LogInformation("INIT_LOGS: " + JsonConvert.SerializeObject(_logs));
                }
            }
            if (!String.IsNullOrEmpty(_session_init_result.ExceptionMessage))
            {
                return null;
            }

            return _common_indic_reader;
        }
        public virtual ICommonIndicationsReader? CreateCommonIndicReader(string communication_interface, IMeterType meterType, SerialPort serialPort, int address, DateTime startDate, DateTime endDate, int energyMonth, int energyYear)
        {
            switch (communication_interface)
            {
                case "GSM":
                    return new Mercury230_234_ModemGSM(
                        meterType: meterType,
                        serialPort: serialPort,
                        address: address,
                        startDate: startDate,
                        endDate: endDate,
                        energyMonth: energyMonth,
                        energyYear: energyYear);
                case "GSM-шлюз":
                    return new Mercury230_234_GatewayGSM(
                        meterType: meterType,
                        serialPort: serialPort,
                        address: address,
                        startDate: startDate,
                        endDate: endDate,
                        energyMonth: energyMonth,
                        energyYear: energyYear);

                default:
                    return null;
            }
        }
        public DateTime? ComputeStartDateForIndicationsReading(string start_date, string last_reading_date)
        {
            DateTime _start = new DateTime();
            DateTime _last = new DateTime();

            if (!DateTime.TryParse(start_date, out _start) || !DateTime.TryParse(last_reading_date, out _last))
                return null;

            var _compare_flag = DateTime.Compare(_start, _last);

            if (_compare_flag < 0)
                return _last;
            else
                return _start;
        }

        public string? DetermineCommunicationInterface(string communication_interface)
        {
            communication_interface = communication_interface.ToLower().Replace(" ", "");
            List<string> COMMUNIC_INTERFACES = GetValidCommunicationInterfaces();
            foreach (var _item in COMMUNIC_INTERFACES)
            {
                if (communication_interface == _item.ToLower().Replace(" ", ""))
                {
                    return _item;
                }
            }

            return null;
        }

        public IMeterType? DetermineMeterType(string meter_type)
        {
            meter_type = meter_type.ToLower().Replace(" ", "");
            var _METER_TYPES = GetValidMeterTypes();
            foreach (var _item in _METER_TYPES)
            {
                if (meter_type.Contains(_item.Name.ToLower()))
                {
                    var _type = meter_type.Replace(_item.Name.ToLower(), "");
                    if (_type == _item.Type.ToLower())
                        return _item;
                }
            }
            return null;
        }
        public virtual List<string> GetValidCommunicationInterfaces()
        {
            return CommonVariables.COMMUNIC_INTERFACES;
        }
        public virtual List<MeterType> GetValidMeterTypes()
        {
            return CommonVariables.METERS_TYPES;
        }
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}
