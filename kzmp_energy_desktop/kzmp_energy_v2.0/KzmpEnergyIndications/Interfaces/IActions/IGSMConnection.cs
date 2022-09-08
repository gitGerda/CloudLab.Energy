using System.IO.Ports;

namespace KzmpEnergyIndicationsLibrary.Interfaces.IActions
{
    public interface IGSMConnection
    {
        Task CloseGSMConnectionAsync(ref SerialPort serialPort);
        Task<SerialPort> CreateGSMConnectionAsync(string simNumber, string bearerType = "71,0,1", int readPauseTime = 500, int readTimeout = 20000);

        public bool CheckSerialPortOpenAndCDHolding(SerialPort serialPort);
    }
}