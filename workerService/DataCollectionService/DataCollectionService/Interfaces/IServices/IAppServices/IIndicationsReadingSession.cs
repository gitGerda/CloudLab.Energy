using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Interfaces.IServices.IAppServices
{
    public interface IIndicationsReadingSession
    {
        /// <summary>
        /// Переподключение к модему
        /// </summary>
        /// <param name="serial_port"></param>
        /// <param name="sim_number"></param>
        /// <param name="shedule_id"></param>
        /// <param name="com_port_name"></param>
        /// <param name="timeout"></param>
        public void ReConnect(ref SerialPort serial_port, string sim_number, int shedule_id, string com_port_name, int timeout);
        /// <summary>
        /// Считывание и отправка в брокер сообщений показаний энергий.
        /// </summary>
        /// <param name="indic_reader"></param>
        /// <param name="rabbit_message_gen"></param>
        /// <param name="shedule_id"></param>
        /// <param name="meter_id"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public Task<bool> ReadAndPublishEnergyIndicationsAsync(ICommonIndicationsReader indic_reader, IRabbitMessageGen rabbit_message_gen, int shedule_id, int meter_id, int month, int year);
        /// <summary>
        /// Считывание и отправка в брокер сообщений показаний профиля мощности со счётчика
        /// </summary>
        /// <param name="indic_reader"></param>
        /// <param name="rabbit_message_gen"></param>
        /// <param name="end_date"></param>
        ///<param name="start_date"></param>
        ///<param name="shedule_id"></param>
        /// <returns></returns>
        public bool ReadAndPublishPowerProfileIndications(ICommonIndicationsReader indic_reader, IRabbitMessageGen rabbit_message_gen, int shedule_id, int meter_id, ref DateTime? start_date, ref DateTime end_date);
        /// <summary>
        /// Инициализация объекта типа IMeterType
        /// </summary>
        /// <param name="meter_type"></param>
        /// <returns></returns>
        public IMeterType? DetermineMeterType(string meter_type);
        /// <summary>
        /// Определение корректного интерфейса связи
        /// </summary>
        /// <param name="communication_interface"></param>
        /// <returns></returns>
        public string? DetermineCommunicationInterface(string communication_interface);
        /// <summary>
        /// Создание объекта считывателя специфичного для communication_interface (интерфейс связи) и инициализация сеанса связи со счётчиком
        /// </summary>
        /// <param name="communication_interface"></param>
        /// <param name="meterType"></param>
        /// <param name="serialPort"></param>
        /// <param name="address"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="energyMonth"></param>
        /// <param name="energyYear"></param>
        /// <param name="shedule_id"></param>
        /// <returns></returns>
        public Task<ICommonIndicationsReader?> InitializeCommunicationSessionAsync(string communication_interface, IMeterType meterType, SerialPort serialPort, int address, DateTime startDate, DateTime endDate, int energyMonth, int energyYear, int shedule_id);
        /// <summary>
        /// Попытка соединения с удалённым модемом
        /// </summary>
        /// <param name="port_name"></param>
        /// <param name="sim_number"></param>
        /// <param name="shedule_id"></param>
        /// <returns></returns>
        public Task<SerialPort?> GetGSMConnectionAsync(string port_name, string sim_number, int shedule_id);
        /// <summary>
        /// Определение даты начала считывания показаний исходя из значения даты указанной пользователем (start_date) 
        /// и значения даты последней записи в базе данных (last_reading_date)  
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="last_reading_date"></param>
        /// <returns></returns>
        public DateTime? ComputeStartDateForIndicationsReading(string start_date, string last_reading_date);
        /// <summary>
        /// Получение текущей даты и времени
        /// </summary>
        /// <returns></returns>
        public DateTime GetCurrentDateTime();

    }
}
