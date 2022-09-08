using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KzmpEnergyIndicationsLibrary.Actions.Communicating.GatewayGSM;
using KzmpEnergyIndicationsLibrary.Devices;
using KzmpEnergyIndicationsLibrary.Interfaces.IDevices;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;

namespace KzmpEnergyIndicationsLibrary.Devices.GatewayGSM
{
    public class Mercury230_234_GatewayGSM : ICommonIndicationsReader
    {
        private Mercury230_234_Communic_GatewayGSM _mercury230_234_Communic;
        public Mercury230_234_GatewayGSM(IMeterType meterType, SerialPort serialPort, int address, DateTime startDate, DateTime endDate, int energyMonth, int energyYear)
        {
            _mercury230_234_Communic = new Mercury230_234_Communic_GatewayGSM(meterType: meterType,
                CrcCalcAlgorithm: CRCLib.CRC.CrcAlgorithms.Crc16Modbus,
                CrcCalcAlgorithmSecond: CRCLib.CRC.CrcAlgorithms.Crc24,
                serialPort: serialPort,
                address: address,
                startDate: startDate,
                endDate: endDate,
                energyMonthNumber: energyMonth, energyYear: energyYear);
        }

        public async Task<SessionInitializationResponse> SessionInitializationAsync()
        {
            SessionInitializationResponse _response = new SessionInitializationResponse();
            try
            {
                //УСТАНОВКА ПАРАМЕТРОВ НА ШЛЮЗЕ
                Console.WriteLine("SET PARAMS");
                Queue<Logs>? gatewayParamLogs = await _mercury230_234_Communic.SetGatewayParametersAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(gatewayParamLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("SET PARAMS OK");

                //ТЕСТ СВЯЗИ СО СЧЁТЧИКОМ
                Console.WriteLine("TEST");
                Queue<Logs>? testConnectionLogs = await _mercury230_234_Communic.TestConnectionGatewayAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(testConnectionLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("TEST OK");

                //АУТЕНТИФИКАЦИЯ И АВТОРИЗАЦИЯ НА СЧЁТЧИКЕ
                Console.WriteLine("AUTH");
                Queue<Logs>? authLogs = await _mercury230_234_Communic.AuthenticateGatewayAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(authLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("AUTH OK");

                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ ВАРИАНТА ИСПОЛНЕНИЯ
                Console.WriteLine("READ VAR ISP");
                Queue<Logs>? execVerReadLogs = await _mercury230_234_Communic.ExecutionVersionReadGatewayAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(execVerReadLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("READ VAR ISP OK");

                //ЗАПРОС НА ПОЛУЧЕНИЕ ДАННЫХ О ПОСЛЕДНЕЙ ЗАПИСИ СЧЁТЧИКА
                Console.WriteLine("LAST RECORD GET REQUEST");
                Queue<Logs>? lastRecordLogs = await _mercury230_234_Communic.GetLastRecordOfMeterGatewayAsync();
                _response.LogsQueue = DevicesCommon.JoinTwoQueuesHook(lastRecordLogs, _response.LogsQueue) ?? _response.LogsQueue;
                Console.WriteLine("LAST RECORD GET REQUEST OK");
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
            return await _mercury230_234_Communic.GetPowerProfileRecordLocalGatewayAsync();
        }
        public async Task<EnergyRecordResponse> GetEnergyRecordAsync(int month, int year)
        {
            return await _mercury230_234_Communic.GetEnergyRecordResponseGatewayLocalAsync(month_number: month, year_number: year);
        }
    }
}
