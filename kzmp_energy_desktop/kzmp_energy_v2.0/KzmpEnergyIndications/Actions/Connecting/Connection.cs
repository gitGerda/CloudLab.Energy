using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using KzmpEnergyIndicationsLibrary.Actions.Communicating;
using KzmpEnergyIndicationsLibrary.Models.Communicating;
using KzmpEnergyIndicationsLibrary.Interfaces.IActions;

namespace KzmpEnergyIndicationsLibrary.Actions.Connecting
{
    public class Connection : IGSMConnection
    {
        private SerialPort _serial_port;
        public SerialPort serial_port
        {
            get => _serial_port;
            private set => _serial_port = value;
        }
        public Connection(string portName, int baudrate = 9600, int readTimeout = 5000, Parity parity = Parity.None, bool dtrEnable = true, bool rtsEnable = true, StopBits stopBits = StopBits.One, int dataBits = 8, Handshake handshake = Handshake.None)
        {
            serial_port = new SerialPort();
            serial_port.PortName = portName;
            serial_port.BaudRate = baudrate;
            serial_port.ReadTimeout = readTimeout;
            serial_port.Parity = parity;
            serial_port.DtrEnable = dtrEnable;
            serial_port.RtsEnable = rtsEnable;
            serial_port.StopBits = stopBits;
            serial_port.DataBits = dataBits;
            serial_port.Handshake = handshake;

            Console.WriteLine("SERIAL PORT OBJECT CREATING");
            try
            {
                serial_port.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("SERIAL PORT OPENED");
        }

        public async Task<SerialPort> CreateGSMConnectionAsync(string simNumber, string bearerType = "71,0,1", int readPauseTime = 500, int readTimeout = 20000)
        {
            Communication communication = new Communication();

            if (serial_port.IsOpen)
            {
                try
                {
                    await WriteGSMHookAsync(serial_port, "ATZ");
                    await WriteGSMHookAsync(serial_port, "ATE0");
                    await WriteGSMHookAsync(serial_port, "AT+CBST=", bearerType);

                    await WriteGSMHookAsync(serial_port, "ATD", simNumber, false);
                    GSMReadingResponse response = await communication.ReadGSMResponseAsync(serialPort: serial_port,
                        responseSize: 5,
                        pauseTime: readPauseTime,
                        timeOver: readTimeout,
                        connectionCreatingFlag: true);

                    if (response.TimeOverFlag)
                    {
                        //serialPort.Close();
                        throw new Exception("Warning: No response");
                    };

                    string responseStr = Encoding.UTF8.GetString(response.Data.ToArray());

                    if (responseStr.Contains("CO") || responseStr.Contains("OK"))
                    {
                        return serial_port;
                    }
                    else if (responseStr.Contains("BU"))
                    {
                        //serialPort.Close();
                        //await CloseGSMConnectionAsync(serial_port);
                        throw new Exception("Warning: BUSY");
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                StringBuilder _sb = new StringBuilder();
                try
                {
                    Console.WriteLine("Trying to reopen port....");
                    _sb.AppendLine("Trying to reopen port....");

                    serial_port.Open();
                    Console.WriteLine("Port succesfully opened");
                    _sb.AppendLine("Port succesfully opened");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2.Message);
                    _sb.AppendLine(ex2.Message);
                }
                _sb.AppendLine("Warning: Port could not be opened and a rediscovery was carried out");
                throw new Exception(_sb.ToString());
            }

            throw new Exception("Error");
        }

        public Task CloseGSMConnectionAsync(ref SerialPort serialPort)
        {
            if (serialPort.IsOpen && serialPort.CDHolding)
            {
                serialPort.DiscardInBuffer();
                Task.Delay(1000).Wait();
                serialPort.Write("+++");
                Task.Delay(2000).Wait();
                serialPort.Write("ATH\r");
                Task.Delay(200).Wait();
            }
            return Task.CompletedTask;
        }

        private async Task WriteGSMHookAsync(SerialPort serialPort, string cmd, string param = "", bool checkOK = true)
        {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            serialPort.Write(cmd + param + "\r");
            await Task.Delay(100);

            if (checkOK)
            {
                string response = serialPort.ReadExisting();
                if (!response.Contains("OK"))
                    throw new Exception($"Error: {cmd + param} - {response}");
            }
        }
        public bool CheckSerialPortOpenAndCDHolding(SerialPort serialPort)
        {
            if (!serialPort.IsOpen || !serialPort.CDHolding)
                return false;

            return true;
        }

    }
}
