using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace KZMP_ENERGY.monthEnergy
{
    public class energy
    {
        public KZMP_ENERGY.FormPowerProfile powerProfForm;
        public recordToDatabase recordClass;

        public SerialPort portLocal;
        byte mercAddress;
        string monthNumber;
        string monthName;
        string currentYear;
        int meterID;

        bool respCrcCheck = false;
        int repeatCounter = 0;

        int timeOver = 500;
        bool timeOverFlag = false;

        public energy(byte mercAddress,int meterId, string month)
        {
            this.portLocal = FormPowerProfile.port;
            this.mercAddress = mercAddress;
            //this.powerProfForm = powPrForm;
            this.meterID = meterId;

            switch (month) 
            {
                case "1. Январь": { monthNumber = "1"; monthName = "Январь"; break; }
                case "2. Февраль": { monthNumber = "2"; monthName = "Февраль"; break; }
                case "3. Март": { monthNumber = "3"; monthName = "Март"; break; }
                case "4. Апрель": { monthNumber = "4"; monthName = "Апрель"; break; }
                case "5. Май": { monthNumber = "5"; monthName = "Май"; break; }
                case "6. Июнь": { monthNumber = "6"; monthName = "Июнь"; break; }
                case "7. Июль": { monthNumber = "7"; monthName = "Июль"; break; }
                case "8. Август": { monthNumber = "8"; monthName = "Август"; break; }
                case "9. Сентябрь": { monthNumber = "9"; monthName = "Сентябрь"; break; }
                case "10. Октябрь": { monthNumber = "10"; monthName = "Октябрь"; break; }
                case "11. Ноябрь": { monthNumber = "11"; monthName = "Ноябрь"; break; }
                case "12. Декабрь": { monthNumber = "12"; monthName = "Декабрь"; break; }
            }

            currentYear = DateTime.Now.Year.ToString();
            recordClass = new recordToDatabase();
        }

        public string yearFieldInRecord()
        {
            int currentMonthNumber = DateTime.Now.Month;

            if(currentMonthNumber > Convert.ToInt32(monthNumber))
            {
                return DateTime.Now.Year.ToString();
            }
            else
            {
                return (DateTime.Now.Year - 1).ToString();
            }
        }
        public async void getMonthEnergy(bool gatewayFlagGlobal)
        {
            recordClass.meterId = meterID.ToString();
            recordClass.year = yearFieldInRecord();
            recordClass.month = monthName;

            //powerProfForm.richTextBox_conStatus2.AppendText("\n-------------------------------------------------------------------------------------------------\nЧТЕНИЕ ПОКАЗАНИЙ НАКОПЛЕННОЙ ЭНЕРГИИ...\n-------------------------------------------------------------------------------------------------\n");
            //powerProfForm.richTextBox_conStatus2.ScrollToCaret();

            string b = "B";
            string b2 = "B";
            int nextMonth = 0;
           
            switch (monthNumber)
            {
                case "10": { b = b + "A"; nextMonth = 11;b2 = b2+"B"; break; }
                case "11": { b = b + "B"; nextMonth = 12; b2 = b2 + "C"; break; }
                case "12": { b = b + "C"; nextMonth = 1; b2 = b2 + "1"; break; }
                default: { 
                        b = b + monthNumber; 
                        nextMonth = Convert.ToInt32(monthNumber) + 1;
                        b2 = b2 + Convert.ToString(nextMonth);
                        break; 
                    }
            }

            //START_VALUE
            byte[] request_crc = new byte[4] { mercAddress, 0x05, Convert.ToByte(b, 16), 0x00 };

            byte[] crc = KZMP_ENERGY.FormPowerProfile.CalculateCrc16Modbus(request_crc);

            byte[] request = new byte[6] { mercAddress, 0x05, Convert.ToByte(b, 16), 0x00, crc[0], crc[1] };

            byte[] request_crc_end = request_crc;
            request_crc_end[2] = Convert.ToByte(b2, 16);
            crc = KZMP_ENERGY.FormPowerProfile.CalculateCrc16Modbus(request_crc_end);
            //request[2]

            repeatCounter = 0;
            respCrcCheck = false;

            if (!gatewayFlagGlobal)
            {
                while (!respCrcCheck && repeatCounter < 4)
                {
                    Task wr= Task.Run(() => write(request, ref portLocal));
                    wr.Wait();
                    Task re = Task.Run(() => read(ref portLocal, true, recordClass.month, recordClass.year, recordClass.meterId, false,
                        out recordClass.startValue));
                    re.Wait();
                    repeatCounter++;
                }
            }
            else
            {
                /*while (!respCrcCheck && repeatCounter < 4)
                {
                    await Task.Run(() => write(request, portLocal));
                    await Task.Run(() => read(portLocal, true, recordClass.month, recordClass.year, recordClass.meterId, false,
                        out recordClass.startValue));

                    repeatCounter++;
                }*/
                while(!respCrcCheck && repeatCounter<4)
                {
                    Task wr= Task.Run(() => gatewayFunc.gatewayWrite(request,ref portLocal));
                    wr.Wait();
                    Task re= Task.Run(() => read(ref portLocal, true, recordClass.month, recordClass.year, recordClass.meterId, true, out recordClass.startValue));
                    re.Wait();

                    repeatCounter++;
                }
            }

            if(repeatCounter < 4 )
            {
                //END_VALUE
                request_crc[2] = Convert.ToByte(b2, 16);
                crc = KZMP_ENERGY.FormPowerProfile.CalculateCrc16Modbus(request_crc);
                request[2] = request_crc[2];
                request[4] = crc[0];
                request[5] = crc[1];

                repeatCounter = 0;
                respCrcCheck = false;

                if (!gatewayFlagGlobal)
                {
                    while (!respCrcCheck && repeatCounter < 4)
                    {
                        Task wr = Task.Run(() => write(request, ref portLocal));
                        wr.Wait();
                        Task re = Task.Run(() => read(ref portLocal, false, recordClass.month, recordClass.year, recordClass.meterId, false,
                            out recordClass.endValue));
                        re.Wait();

                        repeatCounter++;
                    }
                }
                else 
                {
                    while (!respCrcCheck && repeatCounter < 4)
                    {
                        Task wr = Task.Run(() => gatewayFunc.gatewayWrite(request, ref portLocal));
                        wr.Wait();
                        Task re = Task.Run(() => read(ref portLocal, false, recordClass.month, recordClass.year, recordClass.meterId, true, out recordClass.endValue));
                        re.Wait();

                        repeatCounter++;
                    }
                }

                if(repeatCounter<4)
                {
                    string totalValue = Convert.ToString(recordClass.endValue - recordClass.startValue);
                    functionsToDatabase.calculateTotal(ref KZMP_ENERGY.FormPowerProfile.connection, recordClass.meterId, recordClass.year, recordClass.month, totalValue);
                }
            }

        }

        public void write(byte[] mes, ref SerialPort port)
        {
            if (port.CDHolding)
            {
                try
                {
                    port.DiscardInBuffer();
                    port.Write(mes, 0, mes.Length);

                    /*powerProfForm.richTextBox_conStatus2.AppendText(@"# Запрос на чтение накопленной энергии отправлен");
                    powerProfForm.richTextBox_conStatus2.ScrollToCaret();*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                     ex.Message,
                     "kzmp_energy notification",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }
            }
            else
            {
                MessageBox.Show(
                     "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                     "kzmp_energy notification",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }
        }
        public void read(ref SerialPort port,bool startValueFlag, string monthRF, string yearRF, string meterID_RF,bool gatewayFlag, 
            out double calcValue)
        {
            Thread.Sleep(500);
            List<byte> msg = new List<byte>();
            int count = 0;
            int lng = 0;
            int timeCountIndex = 500;
            timeOver = 500;
            respCrcCheck = false;
            calcValue = 0;

            int responseLength = 19;

            if(gatewayFlag) { responseLength = 28; }

            while(lng < responseLength)
            {
                if (!port.CDHolding)
                {
                    MessageBox.Show(
                    "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                    "kzmp_energy notification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    Application.Restart();
                }
                count = port.BytesToRead;
                if (count != 0)
                {
                    byte[] ByteArray = new byte[count];
                    port.Read(ByteArray, 0, count);
                    foreach (byte f in ByteArray)
                    {
                        msg.Add(f);
                    }
                    lng = msg.Count;
                }
                Thread.Sleep(500);
                timeCountIndex = timeCountIndex + 500;
                timeOver = timeOver + 500;

                if (timeOver > 5000)
                {
                    timeOverFlag = true;

                    break;
                }

                if (lng >= responseLength)
                {
                    timeOverFlag = false;
                }
            }

            if (!gatewayFlag)
            {
                if (!timeOverFlag)
                {
                    byte[] check_crc_massa = new byte[] { msg[0], msg[1], msg[2], msg[3], msg[4], msg[5], msg[6], msg[7], msg[8], msg[9], msg[10], msg[11], msg[12], msg[13], msg[14], msg[15], msg[16] };

                    byte[] crc_out = KZMP_ENERGY.FormPowerProfile.CalculateCrc16Modbus(check_crc_massa);

                    if (crc_out[0] == msg[17] && crc_out[1] == msg[18])
                    {
                        respCrcCheck = true;

                        List<string> energyBytesList0 = new List<string>() { msg[1].ToString("X"), msg[2].ToString("X"),
                        msg[3].ToString("X"), msg[4].ToString("X") };

                        List<string> energyBytesList = new List<string>();

                        foreach (string energyByte in energyBytesList0)
                        {
                            string byteEner = energyByte;

                            foreach (string a in KZMP_ENERGY.FormPowerProfile.bagList)
                            {
                                if (energyByte == a)
                                {
                                    int indexOfbyte = energyBytesList0.IndexOf(energyByte);
                                    byteEner = energyBytesList0[indexOfbyte].Insert(0, "0");
                                }
                            }

                            energyBytesList.Add(byteEner);
                        }

                        string energyBytesStr = energyBytesList[1] + energyBytesList[0] + energyBytesList[3] + energyBytesList[2];
                        long energuBytesInt = Convert.ToInt64(energyBytesStr, 16);
                        double energyBytesFl = Convert.ToSingle(energuBytesInt) / 1000;
                        calcValue = energyBytesFl;

                        if (startValueFlag)
                        {
                            if (functionsToDatabase.checkExistence(ref KZMP_ENERGY.FormPowerProfile.connection,
                                meterID_RF, yearRF, monthRF))
                            {
                                functionsToDatabase.updateFunc(ref KZMP_ENERGY.FormPowerProfile.connection,
                                     meterID_RF, yearRF, monthRF, Convert.ToString(energyBytesFl), "0", true);
                            }
                            else
                            {
                                functionsToDatabase.insertFunc(ref KZMP_ENERGY.FormPowerProfile.connection,
                                     meterID_RF, yearRF, monthRF, Convert.ToString(energyBytesFl), "0");
                            }
                        }
                        else
                        {
                            functionsToDatabase.updateFunc(ref KZMP_ENERGY.FormPowerProfile.connection,
                                     meterID_RF, yearRF, monthRF, "0", Convert.ToString(energyBytesFl), false);
                        }


                        /*powerProfForm.richTextBox_conStatus2.AppendText(@"# Ответ успешно получен и обработан!");
                        powerProfForm.richTextBox_conStatus2.ScrollToCaret();*/
                    }
                    else
                    {
                        respCrcCheck = false;
                    }
                }
            }
            else
            {
                if (!timeOverFlag)
                {
                    byte[] check_crc_massa = new byte[] { msg[8], msg[9], msg[10], msg[11], msg[12], msg[13], msg[14], msg[15], msg[16], msg[17], msg[18], msg[19], msg[20], msg[21], msg[22], msg[23], msg[24] };
                    

                    byte[] crc_out = KZMP_ENERGY.FormPowerProfile.CalculateCrc16Modbus(check_crc_massa);

                    if (crc_out[0] == msg[25] && crc_out[1] == msg[26])
                    {
                        respCrcCheck = true;

                        List<string> energyBytesList0 = new List<string>() { msg[9].ToString("X"), msg[10].ToString("X"),
                        msg[11].ToString("X"), msg[12].ToString("X") };

                        List<string> energyBytesList = new List<string>();

                        foreach (string energyByte in energyBytesList0)
                        {
                            string ByteEner = energyByte;

                            foreach (string a in KZMP_ENERGY.FormPowerProfile.bagList)
                            {
                                if (energyByte == a)
                                {
                                    int indexOfbyte = energyBytesList0.IndexOf(energyByte);
                                    ByteEner = energyBytesList0[indexOfbyte].Insert(0, "0");
                                }
                            }
                            energyBytesList.Add(ByteEner);
                        }

                        string energyBytesStr = energyBytesList[1] + energyBytesList[0] + energyBytesList[3] + energyBytesList[2];
                        long energuBytesInt = Convert.ToInt64(energyBytesStr, 16);
                        double energyBytesFl = Convert.ToSingle(energuBytesInt) / 1000;
                        calcValue = energyBytesFl;

                        if (startValueFlag)
                        {
                            if (functionsToDatabase.checkExistence(ref KZMP_ENERGY.FormPowerProfile.connection,
                                meterID_RF, yearRF, monthRF))
                            {
                                functionsToDatabase.updateFunc(ref KZMP_ENERGY.FormPowerProfile.connection,
                                     meterID_RF, yearRF, monthRF, Convert.ToString(energyBytesFl), "0", true);
                            }
                            else
                            {
                                functionsToDatabase.insertFunc(ref KZMP_ENERGY.FormPowerProfile.connection,
                                     meterID_RF, yearRF, monthRF, Convert.ToString(energyBytesFl), "0");
                            }
                        }
                        else
                        {
                            functionsToDatabase.updateFunc(ref KZMP_ENERGY.FormPowerProfile.connection,
                                     meterID_RF, yearRF, monthRF, "0", Convert.ToString(energyBytesFl), false);
                        }


                        /*powerProfForm.richTextBox_conStatus2.AppendText(@"# Ответ успешно получен и обработан!");
                        powerProfForm.richTextBox_conStatus2.ScrollToCaret();*/
                    }
                    else
                    {
                        respCrcCheck = false;
                    }
                }
            }
        }
    }
}
