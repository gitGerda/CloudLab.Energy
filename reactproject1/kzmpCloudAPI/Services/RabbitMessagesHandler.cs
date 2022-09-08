using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Components.General;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Interfaces;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Text;
using static kzmpCloudAPI.Components.General.AppConsts;

namespace kzmpCloudAPI.Services
{
    public class RabbitMessagesHandler : IRabbitMessagesHandler
    {
        kzmp_energyContext _database;
        public RabbitMessagesHandler(IConfiguration configuration)
        {
            _database = _CreateDatabase(configuration);
        }
        internal virtual kzmp_energyContext _CreateDatabase(IConfiguration configuration)
        {
            var databaseConnectionString = configuration.GetConnectionString("DefaultConnection");
            return new kzmp_energyContext(new DbContextOptionsBuilder<kzmp_energyContext>().UseMySql(databaseConnectionString, ServerVersion.Parse("8.0.29-mysql")).Options);
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

        public broker_messages_types? GetTypeOfBrokerMessage(string message)
        {
            JsonSchemaGenerator _schema_generator = new JsonSchemaGenerator();
            JsonSchema _shedule_log_schema = _schema_generator.Generate(typeof(SheduleLog));
            JsonSchema _power_profiles_broker_mesasge_schema = _schema_generator.Generate(typeof(PowerProfilesBrokerMessage));
            JsonSchema _broker_task_message = _schema_generator.Generate(typeof(BrokerTaskMessage));
            JsonSchema _energy_response_message_schema = _schema_generator.Generate(typeof(EnergyRecordResponse));

            try
            {
                var _parsed_message = JObject.Parse(message);

                if (_parsed_message.IsValid(_shedule_log_schema))
                    return broker_messages_types.shedule_log_type;
                else if (_parsed_message.IsValid(_power_profiles_broker_mesasge_schema))
                    return broker_messages_types.power_profiles_broker_message_type;
                else if (_parsed_message.IsValid(_broker_task_message))
                    return broker_messages_types.broker_task_message_type;
                else if (_parsed_message.IsValid(_energy_response_message_schema))
                    return broker_messages_types.energy_response_message_type;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public bool HandleEnergyResponseTypeMessage(EnergyRecordResponse message)
        {
            //Проверка существования в бд записи с таким же meterId, month, year
            if (_database.EnergyTables.Where(t => t.MeterId == message.meter_id && t.Month == message.MonthNumber.ToString() && t.Year == message.Year).Any())
            {
                var _db_record = _database.EnergyTables.Where(t => t.MeterId == message.meter_id && t.Month == message.MonthNumber.ToString() && t.Year == message.Year).Select(t => t).First();

                _db_record.StartValue = message.StartValue.ToString();
                _db_record.EndValue = message.EndValue.ToString();
                _db_record.Total = message.TotalValue.ToString();
                _db_record.Date = _GetCurrentDateTime().ToString();

                _database.EnergyTables.Update(_db_record);
            }
            else
            {
                var _energy_table_record = new EnergyTable()
                {
                    MeterId = message?.meter_id ?? -1,
                    Address = message.MeterAddress,
                    StartValue = message.StartValue.ToString(),
                    EndValue = message.EndValue.ToString(),
                    Month = message.MonthNumber.ToString(),
                    Year = message.Year,
                    Total = message.TotalValue.ToString(),
                    Date = _GetCurrentDateTime().ToString()
                };
                _database.EnergyTables.Add(_energy_table_record);
            }

            if (_database.SaveChanges() == 1)
                return true;
            else
                return false;
        }
        internal virtual DateTime _GetCurrentDateTime()
        {
            return DateTime.Now;
        }
        public bool HandleBrokerTaskTypeMessage(BrokerTaskMessage message)
        {
            DateTime _last_date_time;
            if (!DateTime.TryParse(message.start_date, out _last_date_time))
                return false;

            var _failed_task = new FailedIndicationsTask()
            {
                SheduleId = message.shedule_id ?? -1,
                MeterId = message.meter_id ?? -1,
                LastDatetime = _last_date_time
            };

            _database.FailedIndicationsTasks.Add(_failed_task);
            if (_database.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public bool HandlePowerProfilesTypeMessage(PowerProfilesBrokerMessage message)
        {
            var _records_count = message.Records.Count;
            //Определить адрес счётчика
            var _meter_adress = _GetMeterAddress(meter_id: message.meter_id);
            if (_meter_adress == null)
                return false;
            //Для каждого PowerProfileRecord из message создать объект PowerProfileM базы данных
            //и добавить в таблицу PowerProfiles
            while (message.Records.Count != 0)
            {
                var _record = message.Records.Dequeue();
                //Проверка существования в базе данных аналогичной записи (записи с таким же адресом, датой и временем)
                var _record_date = _record.RecordDate?.Date ?? new DateTime(2001, 01, 01).Date;
                var _record_time = _record.RecordDate?.TimeOfDay ?? TimeSpan.Zero;
                if (!_database.PowerProfileMs.Where(t => t.Address == _meter_adress && t.Date == _record_date && t.Time == _record_time).Any())
                {
                    var _power_profile = new PowerProfileM()
                    {
                        Id = message.meter_id,
                        Address = _meter_adress ?? -1,
                        Date = _record_date,
                        Time = _record_time,
                        Pplus = _record.Pplus,
                        Pminus = _record.Pminus,
                        Qplus = _record.Qplus,
                        Qminus = _record.Qminus
                    };
                    _database.PowerProfileMs.Add(_power_profile);
                }
                else
                {
                    _records_count = _records_count - 1;
                }
            }
            //Отправить изменения в базу данных
            var _db_save_result = _database.SaveChanges();

            if (_db_save_result == _records_count)
                return true;
            else
                return false;
        }

        internal int? _GetMeterAddress(int meter_id)
        {
            if (!_database.Meters.Where(t => t.IdMeter == meter_id).Any())
                return null;

            var _meter_adress = _database.Meters.Where(t => t.IdMeter == meter_id)
                                                .Select(t => t.Address)
                                                .First();

            return _meter_adress;
        }

        public bool HandleSheduleLogTypeMessage(SheduleLog message)
        {
            var _db_shedule_log = new ShedulesLog()
            {
                DateTime = message.date_time,
                Description = message.description,
                SheduleId = message.shedule_id,
                Status = message.status
            };
            _database.ShedulesLogs.Add(_db_shedule_log);
            var _count_added_rows = _database.SaveChanges();

            if (_count_added_rows == 1)
                return true;
            else
                return false;
        }
    }
}
