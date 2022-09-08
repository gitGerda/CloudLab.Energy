using DataCollectionService.Components;
using DataCollectionService.Components.Models;
using DataCollectionService.Interfaces.IServices.IAppServices;
using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Interfaces.IActions;
using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnergyIndicConsts = KzmpEnergyIndicationsLibrary.Variables;

namespace DataCollectionService.Services.AppServices
{
    public class IndicationsReader : IIndicationsReader
    {
        private IConfiguration _configuration;
        private ILogger<IndicationsReader> _logger;
        private string _com_port_name;
        private IGSMConnection _gsm_connection;
        private IRabbitMessageGen _rabbit_message_gen;
        private IIndicationsReadingSession _reading_session;
        private readonly int _REPEATS_COUNT_ON_FAILURE;
        internal readonly int _TIMEOUT_AFTER_FAILURE;
        public IndicationsReader(IConfiguration configuration, ILogger<IndicationsReader> logger, IGSMConnection gsm_connection, IRabbitMessageGen rabbit_message_gen, IIndicationsReadingSession reading_session)
        {
            _configuration = configuration;
            _logger = logger;
            _com_port_name = _configuration["DEFAULT_COM_PORT"] ?? "";
            _gsm_connection = gsm_connection;
            _rabbit_message_gen = rabbit_message_gen;
            _reading_session = reading_session;

            if (!Int32.TryParse(_configuration["REPEATS_COUNT_ON_FAILURE"], out _REPEATS_COUNT_ON_FAILURE))
                _REPEATS_COUNT_ON_FAILURE = 20;

            if (!Int32.TryParse(_configuration["TIMEOUT_AFTER_FAILURE_MS"], out _TIMEOUT_AFTER_FAILURE))
                _TIMEOUT_AFTER_FAILURE = 600000; // 10 minutes
        }

        public bool ChangeComPortName(string com_port)
        {
            try
            {
                //todo: Подумать над перезагрузкой приложения при смене com-порта
                _logger.LogWarning($"PREVIOUS COM-PORT CONFIGURATION : {_configuration["DEFAULT_COM_PORT"]}");
                _configuration["DEFAULT_COM_PORT"] = com_port;
                _com_port_name = com_port;

                var _txt = File.ReadAllText(_configuration["APP_CONFIG_FILE_PATH"]);
                var _jObject = JsonConvert.DeserializeObject<JObject>(_txt);
                _jObject["DEFAULT_COM_PORT"] = com_port;
                File.WriteAllText(_configuration["APP_CONFIG_FILE_PATH"], JsonConvert.SerializeObject(_jObject, Formatting.Indented));

                _logger.LogWarning($"NEW COM-PORT CONFIGURATION : {_configuration["DEFAULT_COM_PORT"]}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                //Отправка логов о неудачной попытке чтения в брокер.
                var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                                        description: ex.Message,
                                        status: EnergyIndicConsts.CommonVariables.ERROR_LOG_STATUS,
                                        shedule_id: -1,
                                        date: null);
                _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
            }
            return true;
        }
        public async Task<bool> GetIndications(List<BrokerTaskMessage> broker_message)
        {
            int _shedule_id = broker_message.Select(t => t.shedule_id).FirstOrDefault() ?? -1;
            _logger.LogInformation("NEW MESSAGE: " + JsonConvert.SerializeObject(broker_message, Formatting.Indented));
            //Проверка поля _com_port_name
            if (String.IsNullOrEmpty(_com_port_name))
            {
                var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                    description: "The default COM port could not be determined",
                    status: EnergyIndicConsts.CommonVariables.ERROR_LOG_STATUS,
                    shedule_id: _shedule_id,
                    date: null);
                _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                _logger.LogCritical("The default COM port could not be determined");
                return false;
            }
            //Фильтрация broker_message по полю sim_number и создание очереди листов
            //с одинаковыми sim_number
            var _message_queue = FilterBrokerTaskMessage(broker_message);

            List<BrokerTaskMessage>? _message_list;
            while (_message_queue.TryDequeue(out _message_list))
            {
                var _sim_number = _message_list.Select(t => t.sim_number).FirstOrDefault() ?? "";
                SerialPort? _serial_port = new SerialPort();
                try
                {
                    //Инициализация соединения с удалённым модемом
                    _serial_port = await _reading_session.GetGSMConnectionAsync(_com_port_name, _sim_number, _shedule_id) ?? _serial_port;
                    //Считывание показаний со счётчиков
                    var session_result = await StartIndicationsReadingSession(serial_port: ref _serial_port, reading_task: ref _message_list);

                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    //Отправка логов о неудачной попытке чтения в брокер.
                    var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                                            description: ex.Message,
                                            status: EnergyIndicConsts.CommonVariables.ERROR_LOG_STATUS,
                                            shedule_id: _shedule_id,
                                            date: null);
                    _logger.LogCritical(ex.Message);
                    _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                }
                finally
                {
                    //Закрытие соединения с удалённым модемом
                    int _timeout = 10000;
                    _logger.LogWarning($"CLOSE GSM CONNECTION AND TIMEOUT [{_timeout}]");
                    await _gsm_connection.CloseGSMConnectionAsync(serialPort: ref _serial_port);
                    await SetDelay(_timeout);
                }
            }
            return true;
        }
        public virtual Task SetDelay(int milisec)
        {
            Task.Delay(milisec).Wait();
            return Task.CompletedTask;
        }
        internal (IMeterType?, string?) _define_meter_type_and_communic_interface(string meter_type, string communic_interface, string meter_address, string sim_number)
        {
            var _meter_type_obj = _reading_session.DetermineMeterType(meter_type); //not async
            if (_meter_type_obj == null)
                //Публикация ошибки и переход к следующему таску
                throw new Exception($"The meter type could not be determined. [Meter type:{meter_type}] [Meter adress: {meter_address}] [SIM: {sim_number}]");
            //Определение типа модема (обычный модем или шлюз)
            var _communic_interface = _reading_session.DetermineCommunicationInterface(communic_interface);//not async
            if (_communic_interface == null)
            {
                //Публикация ошибки и переход к следующему таску
                throw new Exception($"The communication interface could not be determined. [Meter type:{meter_type}] [Meter adress: {meter_address}] [SIM: {sim_number}] [Communication Interface: {communic_interface}]");
            }

            return (_meter_type_obj, _communic_interface);
        }
        internal (string _start_date, string _end_date) _validate_start_end_date(string start_date, string end_date)
        {
            //Валидация стартовой и конечной дат
            if (String.IsNullOrEmpty(start_date))
                start_date = _reading_session.GetCurrentDateTime().ToString();//not async
            if (String.IsNullOrEmpty(end_date))
                end_date = start_date;

            return (start_date, end_date);
        }
        internal Queue<MonthYearEnergyTask> GetEnergyReadingTask(DateTime start_date, DateTime end_date)
        {
            start_date = new DateTime(start_date.Year, start_date.Month, 1);
            end_date = new DateTime(end_date.Year, end_date.Month, 1);

            Queue<MonthYearEnergyTask> queue_tasks = new Queue<MonthYearEnergyTask>();
            queue_tasks.Enqueue(new MonthYearEnergyTask()
            {
                month = start_date.Month,
                year = start_date.Year
            });
            while (true)
            {
                start_date = start_date.AddMonths(1);
                if (DateTime.Compare(start_date, end_date) < 0)
                {
                    queue_tasks.Enqueue(new MonthYearEnergyTask()
                    {
                        month = start_date.Month,
                        year = start_date.Year
                    });
                }
                else
                {
                    break;
                };
            }
            return queue_tasks;
        }
        internal bool ReadPowerProfileAndEnergy(string _communic_interface, IMeterType _meter_type_obj, DateTime? _energy_month_year_date, ref SerialPort serial_port, ref BrokerTaskMessage _task)
        {
            DateTime _end_date = _reading_session.GetCurrentDateTime();
            //DateTime _end_date = new DateTime(2022, 07, 01, 05, 00, 00);
            bool POWER_PROFILE_READED = false;
            bool ENERGY_READED = false;
            //Формирование очереди
            Queue<MonthYearEnergyTask> _energy_reading_tasks = GetEnergyReadingTask(_energy_month_year_date ?? _end_date, _end_date);
            _logger.LogInformation("Задание на считывание энергии: " + JsonConvert.SerializeObject(_energy_reading_tasks, Formatting.Indented));

            _logger.LogInformation($"Repeats count on failure = {_REPEATS_COUNT_ON_FAILURE}");
            for (int i = 0; i < _REPEATS_COUNT_ON_FAILURE; i++)
            {
                //Определение начальной даты для считывания показаний (StartDate в InitializeCommunicationSession)
                DateTime? _start_date = _reading_session.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001"); // not async
                if (_start_date == null)
                    //Публикация ошибки и переход к следующему таску
                    throw new Exception($"Could not compute the start date for reading indications. [Meter type:{_task.meter_type}] [Meter adress: {_task.meter_address}] [SIM: {_task.sim_number}] [Communication Interface: {_task.communic_interface}]");
                else
                {
                    _task.start_date = _start_date?.ToString() ?? _task.start_date;
                    //Если стартовая дата равна или больше текущего времени, то переход к следующему таску
                    if (DateTime.Compare(_start_date ?? DateTime.Now, DateTime.Now) >= 0)
                    {
                        return false;
                    }
                }
                //Создание соответствущего интерфейсу связи объекта считывателя  
                //Инициализация соединения с счётчиком
                ICommonIndicationsReader? _common_indic_reader = _reading_session.InitializeCommunicationSessionAsync(
                   communication_interface: _communic_interface,
                   meterType: _meter_type_obj,
                   serialPort: serial_port,
                   address: Convert.ToInt32(_task.meter_address),
                   startDate: _start_date ?? _end_date,
                   endDate: _end_date,
                   energyMonth: _energy_month_year_date?.Month ?? -1,
                   energyYear: _energy_month_year_date?.Year ?? -1,
                   shedule_id: _task.shedule_id ?? -1).Result;

                //Если возникли ошибки на этапе инициализации сеанса - закрытие соединения, установка таймаута,
                //снова открытие соединения и попытка инициализации
                if (_common_indic_reader == null)
                {
                    _reading_session.ReConnect(serial_port: ref serial_port, sim_number: _task.sim_number ?? "", shedule_id: _task.shedule_id ?? -1, com_port_name: _com_port_name, timeout: _TIMEOUT_AFTER_FAILURE);
                    continue;
                }
                //При успешной инициализации сеанса подсчитываем количество часов между датой датой начала (startDate)
                //и конечной датой (endDate) и начинаем считывание профиля мощности и отправку показаний в брокер
                if (!POWER_PROFILE_READED)
                {
                    if (!_reading_session.ReadAndPublishPowerProfileIndications(indic_reader: _common_indic_reader,
                        rabbit_message_gen: _rabbit_message_gen,
                        shedule_id: _task.shedule_id ?? -1,
                        meter_id: _task.meter_id ?? -1,
                        start_date: ref _start_date,
                        end_date: ref _end_date))
                    {
                        //Если возникает ошибка при считывании показаний, меняем стартовую дату таска на дату последнего прочитанного
                        //показания, делаем реконект к модему, повторяем цикл
                        _task.start_date = _start_date.ToString() ?? _task.start_date;
                        _reading_session.ReConnect(serial_port: ref serial_port, sim_number: _task.sim_number ?? "", shedule_id: _task.shedule_id ?? -1, com_port_name: _com_port_name, timeout: _TIMEOUT_AFTER_FAILURE);
                        continue;
                    }
                    else
                    {
                        _task.start_date = _start_date.ToString() ?? _task.start_date;
                        POWER_PROFILE_READED = true;
                    }
                }
                if (POWER_PROFILE_READED)
                {
                    // После окончания считывания профиля мощности приступаем к считыванию показаний энергий
                    try
                    {
                        //Считывание энергии нужно проводить не только за месяц стартовой даты, так как между стартовой и конечными датами может быть несколько месяцев за которые необходимо снять энергию. Исходя из стартовой и конечных дат можно создать набор месяцев. Из этого набора можно удалить месяц равный текущему месяцу. За все остальные месяцы показания снимаем. + ко всему этому в апи необходимо исключить добавление в бд нескольких записей энергий за один и тот же месяц. 

                        bool _loop_flag = true;
                        while (_loop_flag == true)
                        {
                            MonthYearEnergyTask _energy_task = new MonthYearEnergyTask();
                            if (!_energy_reading_tasks.TryPeek(out _energy_task))
                            {
                                ENERGY_READED = true;
                                return true;
                            }
                            _logger.LogInformation("Задание на чтение энергии отправлено: " + JsonConvert.SerializeObject(_energy_task, Formatting.Indented));

                            try
                            {

                                if (_reading_session.ReadAndPublishEnergyIndicationsAsync(indic_reader: _common_indic_reader,
                                    rabbit_message_gen: _rabbit_message_gen,
                                    shedule_id: _task.shedule_id ?? -1,
                                    meter_id: _task.meter_id ?? -1,
                                    month: _energy_task.month,
                                    year: _energy_task.year).Result)
                                {
                                    //Если показания энергий считались успешно удаляем из очереди energy_task 
                                    _energy_reading_tasks.Dequeue();
                                    if (_energy_reading_tasks.Count == 0)
                                    {
                                        ENERGY_READED = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    _reading_session.ReConnect(serial_port: ref serial_port, sim_number: _task.sim_number ?? "", shedule_id: _task.shedule_id ?? -1, com_port_name: _com_port_name, timeout: _TIMEOUT_AFTER_FAILURE);
                                    _loop_flag = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                // Если параметры месяца и года для считывания показаний энергий не удовлетворяют
                                // условиям - удаляем из очереди energy_task без отравки логов на сервер,
                                if (ex.Message.Contains("incorrect month or year"))
                                {
                                    _energy_reading_tasks.Dequeue();
                                    continue;
                                }
                                else
                                {
                                    //Иначе отправить логи на сервер. Переход к следующему energy_task
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Иначе отправить логи на сервер. Переход к следующему energy_task
                        _logger.LogCritical("ENERGY INDIC READING EXCEPTION: " + ex.Message);

                        var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                            description: ex.Message,
                            status: EnergyIndicConsts.CommonVariables.ERROR_LOG_STATUS,
                            shedule_id: _task.shedule_id ?? -1,
                            date: null);
                        _logger.LogCritical(ex.Message);
                        _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);

                        _logger.LogCritical("При считывании показаний энергии возникли ошибки. Повтор инициализации сеанса связи...");
                        _reading_session.ReConnect(serial_port: ref serial_port, sim_number: _task.sim_number ?? "", shedule_id: _task.shedule_id ?? -1, com_port_name: _com_port_name, timeout: _TIMEOUT_AFTER_FAILURE);
                        continue;
                    }
                }
            }
            string _mes = "Не удалось считать показания, так как количество попыток считывания исчерпано";
            _logger.LogCritical(_mes);
            throw new Exception(_mes);
        }
        public virtual Task<bool> StartIndicationsReadingSession(ref SerialPort serial_port, ref List<BrokerTaskMessage> reading_task)
        {
            for (int i = 0; i < reading_task.Count; i++)
            {
                BrokerTaskMessage _task = reading_task[i];
                try
                {
                    //Определение типа счётчика и типа интерфейса связи
                    (IMeterType? _meter_type_obj, string? _communic_interface) = _define_meter_type_and_communic_interface(_task?.meter_type ?? "", _task?.communic_interface ?? "", _task?.meter_address ?? "", _task?.sim_number ?? "");
                    //Проверка стартовой даты и даты последнего измерения
                    (_task.start_date, _task.last_indication_datetime) = _validate_start_end_date(_task?.start_date ?? "", _task?.last_indication_datetime ?? "");

                    //Подсчёт даты для инициализации полей energyMonth, energyYear 
                    DateTime? _energy_month_year_date = _reading_session.ComputeStartDateForIndicationsReading(_task.start_date, _task.last_indication_datetime ?? "01.01.2001"); // not async  

                    //Считывание профиля мощности и энергии
                    bool _reading_result_flag = ReadPowerProfileAndEnergy(
                        _communic_interface: _communic_interface ?? "",
                        _meter_type_obj: _meter_type_obj,
                        _energy_month_year_date: _energy_month_year_date,
                        serial_port: ref serial_port,
                        _task: ref _task);

                }
                catch (Exception ex)
                {
                    //Отправка логов о неудачной попытке чтения в брокер.
                    var _shedule_log = _rabbit_message_gen.CreateSheduleLog(
                        description: ex.Message,
                        status: EnergyIndicConsts.CommonVariables.ERROR_LOG_STATUS,
                        shedule_id: _task.shedule_id ?? -1,
                        date: null);
                    _logger.LogCritical(ex.Message);
                    _rabbit_message_gen.PublishMessageToRabbit(_shedule_log);
                    //Отправка информации _message_queue (+ информация о последних прочитанных показаниях счётчиков) 
                    _logger.LogCritical(JsonConvert.SerializeObject(_task));
                    _rabbit_message_gen.PublishMessageToRabbit(_task);
                }
                reading_task[i] = _task;
            }
            return Task.FromResult(true);
        }
        public virtual Queue<List<BrokerTaskMessage>> FilterBrokerTaskMessage(List<BrokerTaskMessage> broker_message)
        {
            var result = new Queue<List<BrokerTaskMessage>>();
            if (broker_message.Count == 0)
                return result;

            var _different_sim = broker_message.Select(t => t.sim_number).Distinct().ToList();
            foreach (var sim_number in _different_sim)
            {
                var _message_list_by_sim = broker_message.Where(t => t.sim_number == sim_number).Select(t => t).ToList();
                result.Enqueue(_message_list_by_sim);
            }

            return result;
        }
        public int? GetTypeOfBrokerMessage(string message)
        {
            JsonSchemaGenerator _schema_generator = new JsonSchemaGenerator();
            JsonSchema _list_broker_task_message_schema = _schema_generator.Generate(typeof(List<BrokerTaskMessage>));
            JsonSchema _port_conf_schema = _schema_generator.Generate(typeof(PortConfiguration));

            try
            {
                var _parsed_message = JObject.Parse(message);
                if (_parsed_message.IsValid(_port_conf_schema))
                    return AppConsts.PORT_CONFIGURATION_TYPE;
            }
            catch { }

            try
            {
                var _parsed_message = JArray.Parse(message);
                if (_parsed_message.IsValid(_list_broker_task_message_schema))
                    return AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE;
            }
            catch { }
            throw new Exception("Invalid broker message type");
        }
        public string BrokerMessageToString(byte[] message)
        {
            var _message = Encoding.UTF8.GetString(message);
            return _message;
        }

        public T DeserializeBrokerMessage<T>(string broker_message)
        {
            return JsonConvert.DeserializeObject<T>(broker_message);
        }
    }
}
