using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using KzmpEnergyIndicationsLibrary.Models.Communicating;

namespace KzmpEnergyIndicationsLibrary.Actions.Communicating
{
    internal class Communication
    {
        public async Task<GSMReadingResponse> ReadGSMResponseAsync(SerialPort serialPort, int responseSize, int pauseTime = 500, int timeOver = 5000, bool connectionCreatingFlag = false)
        {
            GSMReadingResponse response = new GSMReadingResponse();
            int readingTime = pauseTime;

            await Task.Delay(pauseTime);

            while (response.Data.Count() < responseSize)
            {
                if (connectionCreatingFlag == false)
                {
                    if (!serialPort.CDHolding)
                        throw new Exception("Error: NO CARRIER (Communication failure)");
                }
                else
                {
                    if (!serialPort.IsOpen)
                        throw new Exception("Error: port is closed (Communication failure)");
                }

                if (serialPort.BytesToRead != 0)
                {
                    var buffer = new byte[serialPort.BytesToRead];
                    serialPort.Read(buffer, 0, buffer.Length);
                    response.Data.AddRange(buffer);
                }

                await Task.Delay(pauseTime);
                readingTime += pauseTime;

                if (readingTime > timeOver)
                {
                    response.TimeOverFlag = true;
                    break;
                }
            }

            return response;
        }

        public void WriteMessageGSM(SerialPort serialPort, byte[] message)
        {
            if (serialPort.CDHolding)
            {
                serialPort.DiscardInBuffer();
                serialPort.Write(message, 0, message.Length);
            }
            else
            {
                serialPort.Close();
                throw new Exception("Error: NO CARRIER");
            }
        }


    }
}
