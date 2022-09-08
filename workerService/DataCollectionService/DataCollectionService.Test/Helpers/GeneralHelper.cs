using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Test.Helpers
{
    public static class GeneralHelper
    {
        public static EnergyRecordResponse _getEnergyRecordResponse()
        {
            var _logs = new Queue<Logs>();
            _logs.Enqueue(_getDefaultLog());
            _logs.Enqueue(_getDefaultLog());
            _logs.Enqueue(_getDefaultLog());

            return new EnergyRecordResponse()
            {
                EndValue = 0,
                MeterAddress = 1,
                MonthNumber = 1,
                StartValue = 0,
                TotalValue = 0,
                Year = 2001,
                Logs = _logs
            };
        }
        public static PowerProfilesBrokerMessage _getPowerProfileBrokerMessage()
        {
            var _records_queue = new Queue<PowerProfileRecord>();
            _records_queue.Enqueue(_getDefaultPowerProfileRecord());
            _records_queue.Enqueue(_getDefaultPowerProfileRecord());
            _records_queue.Enqueue(_getDefaultPowerProfileRecord());

            return new PowerProfilesBrokerMessage()
            {
                meter_id = 1,
                shedule_id = 1,
                Records = _records_queue
            };
        }
        public static PowerProfileRecordResponse _getValidPowerProfileRecordResponse()
        {
            var _records_queue = new Queue<PowerProfileRecord>();
            _records_queue.Enqueue(_getDefaultPowerProfileRecord());
            _records_queue.Enqueue(_getDefaultPowerProfileRecord());
            _records_queue.Enqueue(_getDefaultPowerProfileRecord());

            var _logs_queue = new Queue<Logs>();
            _logs_queue.Enqueue(_getDefaultLog());
            _logs_queue.Enqueue(_getDefaultLog());
            _logs_queue.Enqueue(_getDefaultLog());

            return new PowerProfileRecordResponse()
            {
                Address = "1",
                Logs = _logs_queue,
                Records = _records_queue,
                Exception = ""
            };
        }
        public static Logs _getDefaultLog()
        {
            return new Logs()
            {
                Date = new DateTime(2001, 01, 01, 00, 00, 00),
                Description = "",
                Status = CommonVariables.WARNING_LOG_STATUS
            };
        }
        public static PowerProfileRecord _getDefaultPowerProfileRecord()
        {
            return new PowerProfileRecord()
            {
                Pminus = 0,
                Pplus = 0,
                Qminus = 0,
                Qplus = 0,
                RecordDate = new DateTime(2001, 01, 01, 00, 00, 00)
            };
        }
        public static SessionInitializationResponse _getValidSessionInitResponse()
        {
            var _queue_logs = new Queue<Logs>();
            _queue_logs.Enqueue(new Logs()
            {
                Date = new DateTime(),
                Description = "",
                Status = ""
            });

            return new SessionInitializationResponse()
            {
                ExceptionMessage = "",
                LogsQueue = _queue_logs
            };

        }
        public static string? _getSupportedCommunicationInterface()
        {
            return CommonVariables.COMMUNIC_INTERFACES.First();
        }
        public static MeterType? _getSupportedMeterType()
        {
            return CommonVariables.METERS_TYPES.First();
        }
        public static List<string> _getDefaultCommunicationInterfaces()
        {
            return new List<string>()
            {
                "GSM",
                "GSM-шлюз"
            };
        }
        public static List<MeterType> _getDefaultMetersTypes()
        {
            return new List<MeterType>()
            {
            new MeterType()
            {
                Name="Меркурий",
                Type="234"
            },
            new MeterType()
            {
                Name="Меркурий",
                Type="230"
            }
            };
        }
        public static Queue<List<BrokerTaskMessage>> _getFiltredBySimNumberQueueOfBrokerTaskMessageList()
        {
            var _list1 = new List<BrokerTaskMessage>(){
                    new BrokerTaskMessage(){
                        communic_interface="",
                        last_indication_datetime="",
                        meter_address="",
                        meter_type="",
                        sim_number="1234567890",
                        start_date=""
                    },

                    new BrokerTaskMessage(){
                        communic_interface="",
                        last_indication_datetime="",
                        meter_address="",
                        meter_type="",
                        sim_number="1234567890",
                        start_date=""
                    }
                };
            var _list2 = new List<BrokerTaskMessage>(){
                    new BrokerTaskMessage(){
                        communic_interface="",
                        last_indication_datetime="",
                        meter_address="",
                        meter_type="",
                        sim_number="0987654321",
                        start_date=""
                    },

                    new BrokerTaskMessage(){
                        communic_interface="",
                        last_indication_datetime="",
                        meter_address="",
                        meter_type="",
                        sim_number="0987654321",
                        start_date=""
                    }
                };
            var _result = new Queue<List<BrokerTaskMessage>>();
            _result.Enqueue(_list1);
            _result.Enqueue(_list2);

            return _result;
        }
        public static List<BrokerTaskMessage> _getDefaultBrokerTaskMessageList()
        {
            return new List<BrokerTaskMessage>()
            {
                new BrokerTaskMessage()
                {
                    communic_interface="",
                    last_indication_datetime="01.01.2001",
                    meter_address="0",
                    meter_type="",
                    sim_number="",
                    start_date="01.01.2001",
                    shedule_id=1,
                    meter_id=1
                },

                new BrokerTaskMessage()
                {
                    communic_interface="",
                    last_indication_datetime="01.01.2001",
                    meter_address="0",
                    meter_type="",
                    sim_number="",
                    start_date="01.01.2001",
                    shedule_id=1,
                    meter_id=2
                },

                new BrokerTaskMessage()
                {
                    communic_interface="",
                    last_indication_datetime="01.01.2001",
                    meter_address="0",
                    meter_type="",
                    sim_number="",
                    start_date="01.01.2001",
                    shedule_id=1,
                    meter_id=3
                }
            };
        }

        public static SheduleLog _getDefaultSheduleLog()
        {
            return new SheduleLog()
            {
                date_time = DateTime.Now,
                description = "",
                shedule_id = 1,
                status = "success"
            };
        }

    }
}
