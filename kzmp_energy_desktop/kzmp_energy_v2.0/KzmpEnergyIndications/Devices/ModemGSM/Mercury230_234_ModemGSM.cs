using System.IO.Ports;
using KzmpEnergyIndicationsLibrary.Devices;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Actions.Communicating.ModemGSM;
using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;

namespace KzmpEnergyIndicationsLibrary.Devices.ModemGSM
{
    public class Mercury230_234_ModemGSM : ICommonIndicationsReader
    {
        private Mercury230_234_Communic_ModemGSM mercury230_234Communication;
        public Mercury230_234_ModemGSM(IMeterType meterType, SerialPort serialPort, int address, DateTime startDate, DateTime endDate, int energyMonth, int energyYear)
        {
            mercury230_234Communication = new Mercury230_234_Communic_ModemGSM(CrcCalcAlgorithm: CRCLib.CRC.CrcAlgorithms.Crc16Modbus, serialPort: serialPort, address: address, meterType: meterType, startDate: startDate, endDate: endDate, energyMonthNumber: energyMonth, energyYear: energyYear);
        }
        public async Task<SessionInitializationResponse> SessionInitializationAsync()
        {
            SessionInitializationResponse _response = new SessionInitializationResponse();
            try
            {
                //ТЕСТ СВЯЗИ СО СЧЁТЧИКОМ
                Console.WriteLine("TEST CONNECTION");
                Queue<Logs>? testConnectionLogs = await mercury230_234Communication.TestConnectionAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(testConnectionLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("TEST CONNECTION OK");

                //АУТЕНТИФИКАЦИЯ И АВТОРИЗАЦИЯ НА СЧЁТЧИКЕ
                Console.WriteLine("AUTH");
                Queue<Logs>? authLogs = await mercury230_234Communication.AuthenticateAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(authLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("AUTH OK");

                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ ВАРИАНТА ИСПОЛНЕНИЯ
                Console.WriteLine("EXECUTION VERSION READ");
                Queue<Logs>? executionVariantLogs = await mercury230_234Communication.ExecutionVersionReadAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(executionVariantLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("EXECUTION VERSION READ OK");

                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ О ПОСЛЕДНЕЙ ЗАПИСИ СЧЁТЧИКА
                Console.WriteLine("GET LAST RECORD");
                Queue<Logs>? lastRecordLogs = await mercury230_234Communication.GetLastRecordOfMeterAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(lastRecordLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("GET LAST RECORD OK");
            }
            catch (Exception ex)
            {
                _response.ExceptionMessage = ex.Message;
                _response.LogsQueue.Enqueue(new Logs()
                {
                    Date = DateTime.Now,
                    Status = CommonVariables.ERROR_LOG_STATUS,
                    Description = $"{ex.Message}"
                });
            }

            return _response;
        }
        public async Task<PowerProfileRecordResponse> GetPowerProfileRecordAsync()
        {
            return await mercury230_234Communication.GetPowerProfileRecordLocalAsync();
        }
        public async Task<EnergyRecordResponse> GetEnergyRecordAsync(int month, int year)
        {
            return await mercury230_234Communication.GetEnergyValuesLocalAsync(month_number: month,year_number:year);
        }
    }
}
