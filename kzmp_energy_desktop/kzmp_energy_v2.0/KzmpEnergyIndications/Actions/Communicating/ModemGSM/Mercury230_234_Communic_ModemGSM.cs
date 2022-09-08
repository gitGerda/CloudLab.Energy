using KzmpEnergyIndicationsLibrary.Actions.CRCCalculating;
using KzmpEnergyIndicationsLibrary.Devices;
using KzmpEnergyIndicationsLibrary.Models.Communicating;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;
using System.IO.Ports;


namespace KzmpEnergyIndicationsLibrary.Actions.Communicating.ModemGSM
{
    internal class Mercury230_234_Communic_ModemGSM
    {
        private Communication _communication = new Communication();
        private CRC _CRCCalc = new CRC();
        private CRCLib.CRC.CrcAlgorithms _CrcCalcAlgorithm;
        private SerialPort _serialPort;
        private readonly int _address;
        private readonly byte _hexAddress;
        //постоянная счётчика (A)
        private float _METER_CONSTANT;
        //период интегрирования (T)
        private float _METER_T;
        //лист байтов последней записи на счётчике
        private List<byte>? _LAST_PARAMETR;
        //переменная для установки 3-го байта в запросе 06h для счетчиков меркурий 234.
        private byte _bit17for234 = 0x03;
        private byte powerProfileRequestCode = 0x03;
        private IMeterType _meterType;
        //старший и младший байт адреса записи в памяти счётчика
        private byte _OLDER_BYTE;
        private byte _YOUNG_BYTE;
        //начало и конец периода для снятия показаний
        private DateTime _startDate;
        private DateTime _endDate;
        //месяц и год для снятия показаний энергий
        private int _energyMonthNumber;
        private int _energyYear;
        internal Mercury230_234_Communic_ModemGSM(IMeterType meterType, CRCLib.CRC.CrcAlgorithms CrcCalcAlgorithm, SerialPort serialPort, int address,
            DateTime startDate, DateTime endDate, int energyMonthNumber, int energyYear)
        {
            _CrcCalcAlgorithm = CrcCalcAlgorithm;
            _serialPort = serialPort;
            _address = address;
            _hexAddress = Convert.ToByte(_address);
            _meterType = meterType;
            _startDate = startDate;
            _endDate = endDate;
            _energyMonthNumber = energyMonthNumber;
            _energyYear = energyYear;
        }
        internal async Task<Queue<Logs>> TestConnectionAsync()
        {
            byte hex = Convert.ToByte(_address);
            byte[] meter_connection_test = new byte[4];
            byte[] meter_connection_test_crc = new byte[] { hex, 0x00 };
            byte[] crc = _CRCCalc.CalculateCrc(meter_connection_test_crc, _CrcCalcAlgorithm);
            Queue<Logs> logs = new Queue<Logs>();

            logs.Enqueue(new Logs()
            {
                Date = DateTime.Now,
                Status = CommonVariables.INFO_LOG_STATUS,
                Description = $"Test connection (address: {_address})"
            });

            meter_connection_test[0] = hex;
            meter_connection_test[1] = 0x00;
            meter_connection_test[2] = crc[0];
            meter_connection_test[3] = crc[1];

            for (int i = 0; i < CommonVariables.REPEAT_REQUESTS_COUNT; i++)
            {
                _communication.WriteMessageGSM(_serialPort, meter_connection_test);
                GSMReadingResponse _readingResponse = await _communication.ReadGSMResponseAsync(serialPort: _serialPort,
                    responseSize: 4,
                    pauseTime: 100);

                if (_readingResponse.TimeOverFlag)
                {
                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.WARNING_LOG_STATUS,
                        Description = $"No response on test request (address: {_address})"
                    });
                }
                else
                {
                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.SUCCESS_LOG_STATUS,
                        Description = $"Test connection [OK] (address:{_address})"
                    });
                    return logs;
                }
            }

            throw new Exception($"Error: No response on test connection request (address: {_address})");
        }
        internal async Task<Queue<Logs>> AuthenticateAsync()
        {
            byte hex = Convert.ToByte(_address);
            byte[] ath_meter_crc = new byte[] { hex, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            byte[] crc2 = _CRCCalc.CalculateCrc(ath_meter_crc, _CrcCalcAlgorithm);
            byte[] ath_meter = new byte[11] { hex, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, crc2[0], crc2[1] };
            Queue<Logs> logs = new Queue<Logs>();

            logs.Enqueue(new Logs()
            {
                Date = DateTime.Now,
                Status = CommonVariables.INFO_LOG_STATUS,
                Description = $"Authenticate (address: {_address})"
            });


            for (int i = 0; i < CommonVariables.REPEAT_REQUESTS_COUNT; i++)
            {
                _communication.WriteMessageGSM(serialPort: _serialPort, message: ath_meter);
                GSMReadingResponse _readingResponse = await _communication.ReadGSMResponseAsync(serialPort: _serialPort,
                    responseSize: 4,
                    pauseTime: 100);

                if (_readingResponse.TimeOverFlag)
                {
                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.WARNING_LOG_STATUS,
                        Description = $"No response on authenticate request (meter address: {_address})"
                    });
                }
                else
                {
                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.SUCCESS_LOG_STATUS,
                        Description = $"Authenticate [OK] (address:{_address})"
                    });
                    return logs;
                }
            }

            throw new Exception($"Error: No response on authenticate request (address: {_address})");
        }
        internal async Task<Queue<Logs>> ExecutionVersionReadAsync()
        {
            byte hex = Convert.ToByte(_address);
            byte[] var_isp_crc = new byte[] { hex, 0x08, 0x12 };
            byte[] var_isp_crcR = _CRCCalc.CalculateCrc(var_isp_crc, _CrcCalcAlgorithm);
            byte[] var_isp = new byte[] { hex, 8, 18, var_isp_crcR[0], var_isp_crcR[1] };
            Queue<Logs> logs = new Queue<Logs>();

            logs.Enqueue(new Logs()
            {
                Date = DateTime.Now,
                Status = CommonVariables.INFO_LOG_STATUS,
                Description = $"ExecutionVersionRead (address: {_address})"
            });

            for (int i = 0; i < CommonVariables.REPEAT_REQUESTS_COUNT; i++)
            {
                _communication.WriteMessageGSM(serialPort: _serialPort, message: var_isp);
                GSMReadingResponse _readingResponse = await _communication.ReadGSMResponseAsync(serialPort: _serialPort,
                    responseSize: 9,
                    timeOver: 7000);

                if (_readingResponse.TimeOverFlag)
                {
                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.WARNING_LOG_STATUS,
                        Description = $"No response on ExecutionVersionRead request (meter address: {_address})"
                    });
                }
                else
                {
                    if (_readingResponse.Data.Count != 9)
                        continue;

                    byte[] check_crc_massa = _readingResponse.Data.Take(7).ToArray();
                    byte[] crc_out = _CRCCalc.CalculateCrc(check_crc_massa, _CrcCalcAlgorithm);

                    if (_readingResponse.Data[7] != crc_out[0] || _readingResponse.Data[8] != crc_out[1])
                    {
                        continue;
                    }

                    byte k = _readingResponse.Data[2];
                    byte mask = 0b00001111;
                    int Aindex = k & mask;
                    _METER_CONSTANT = CommonVariables.METERS_CONSTANTS[Aindex];


                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.SUCCESS_LOG_STATUS,
                        Description = $"ExecutionVersionRead [OK] (address:{_address})"
                    });
                    return logs;
                }
            }

            throw new Exception($"Error: No response on ExecutionVersionRead request (address: {_address})");
        }
        internal async Task<Queue<Logs>> GetLastRecordOfMeterAsync()
        {
            byte[] last_parameter_crc = new byte[] { _hexAddress, 0x08, 0x13 };
            byte[] crc3 = _CRCCalc.CalculateCrc(last_parameter_crc, _CrcCalcAlgorithm);
            byte[] last_parameter = new byte[5] { _hexAddress, 8, 19, crc3[0], crc3[1] };
            Queue<Logs> logs = new Queue<Logs>();

            logs.Enqueue(new Logs()
            {
                Date = DateTime.Now,
                Status = CommonVariables.INFO_LOG_STATUS,
                Description = $"GetLastRecordOnMeter (address: {_address})"
            });

            for (int i = 0; i < CommonVariables.REPEAT_REQUESTS_COUNT; i++)
            {
                _communication.WriteMessageGSM(serialPort: _serialPort, message: last_parameter);
                GSMReadingResponse _readingResponse = await _communication.ReadGSMResponseAsync(serialPort: _serialPort,
                    responseSize: 12,
                    timeOver: 7000);

                if (_readingResponse.TimeOverFlag)
                {
                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.WARNING_LOG_STATUS,
                        Description = $"No response on GetLastRecordOnMeter request (meter address: {_address})"
                    });
                }
                else
                {
                    if (_readingResponse.Data.Count != 12)
                        continue;

                    byte[] check_crc_massa = _readingResponse.Data.Take(10).ToArray();
                    byte[] crc_out = _CRCCalc.CalculateCrc(check_crc_massa, _CrcCalcAlgorithm);

                    if (_readingResponse.Data[10] != crc_out[0] || _readingResponse.Data[11] != crc_out[1])
                    {
                        continue;
                    }

                    _METER_T = Convert.ToInt32(_readingResponse.Data[9]);
                    _LAST_PARAMETR = _readingResponse.Data.ToList();

                    //если счетчик типа Меркурий 234, то нужно определить 3-ий байт в 06h запросе (определить 17 бит)
                    if (_meterType.Name == "Меркурий" && _meterType.Type == "234")
                    {
                        string first_str = _LAST_PARAMETR[1].ToString("X");
                        string second_str = _LAST_PARAMETR[2].ToString("X");

                        if ((from t in CommonVariables.BAG_LIST
                             where t == second_str
                             select t).Any())
                        {
                            second_str = second_str.Insert(0, "0");
                        }
                        string general_str = first_str + second_str + "0";
                        int k = Convert.ToInt32(general_str, 16);
                        string general_strBinary = Convert.ToString(k, 2);

                        if (general_strBinary.Length > 16)
                        {
                            int index_in_strBin = general_strBinary.Length - 17;

                            string n = Convert.ToString(general_strBinary[index_in_strBin]);

                            int m = Convert.ToInt32(n);

                            if (m == 0)
                            {
                                _bit17for234 = 0x03;
                            }
                            else
                            {
                                _bit17for234 = 0x83;
                            }
                        }
                        else
                        {
                            _bit17for234 = 0x03;
                        }
                    }

                    CommunicCommon.ComputeOlderYoungBytesForStartDate(_LAST_PARAMETR, _meterType, _startDate, ref _YOUNG_BYTE, ref _OLDER_BYTE, ref _bit17for234);

                    logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.SUCCESS_LOG_STATUS,
                        Description = $"GetLastRecordOnMeter [OK] (address:{_address})"
                    });
                    return logs;
                }
            }

            throw new Exception($"Error: No response on GetLastRecordOnMeter request (address: {_address})");
        }
        internal async Task<PowerProfileRecordResponse> GetPowerProfileRecordLocalAsync()
        {
            PowerProfileRecordResponse _response = new PowerProfileRecordResponse();
            DateTime _dateTimeOfRecord;

            if (_meterType.Name == "Меркурий" && _meterType.Type == "234")
                powerProfileRequestCode = _bit17for234;

            byte[] power_crc = new byte[] { _hexAddress, 0x06, powerProfileRequestCode, _OLDER_BYTE, _YOUNG_BYTE, 0x1e };
            byte[] crc = _CRCCalc.CalculateCrc(power_crc, _CrcCalcAlgorithm);
            byte[] power = new byte[] { _hexAddress, 0x06, powerProfileRequestCode, _OLDER_BYTE, _YOUNG_BYTE, 0x1e, crc[0], crc[1] };

            for (int i = 0; i < CommonVariables.REPEAT_REQUESTS_COUNT; i++)
            {
                _communication.WriteMessageGSM(_serialPort, power);
                GSMReadingResponse _readingResponse = await _communication.ReadGSMResponseAsync(serialPort: _serialPort,
                    responseSize: 33,
                    timeOver: 7000);

                if (_readingResponse.TimeOverFlag)
                {
                    _response.Logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.WARNING_LOG_STATUS,
                        Description = $"No response on GetPowerProfileRecord request (address: {_address})"
                    });
                }
                else
                {
                    byte[] check_crc_massa = _readingResponse.Data.Take(31).ToArray();
                    byte[] crc_out = _CRCCalc.CalculateCrc(check_crc_massa, _CrcCalcAlgorithm);

                    if (_readingResponse.Data[31] != crc_out[0] || _readingResponse.Data[32] != crc_out[1])
                    {
                        _response.Logs.Enqueue(new Logs()
                        {
                            Date = DateTime.Now,
                            Status = CommonVariables.WARNING_LOG_STATUS,
                            Description = $"Error checking hash amounts (address:{_address}, date:{Convert.ToInt32(_readingResponse.Data[4]).ToString("X")}.{Convert.ToInt32(_readingResponse.Data[5]).ToString("X")}.{Convert.ToInt32(_readingResponse.Data[6]).ToString("X")} {Convert.ToInt32(_readingResponse.Data[2]).ToString("X")}:{Convert.ToInt32(_readingResponse.Data[3]).ToString("X")})"
                        });
                        i = i - 1;
                        continue;
                    }

                    //1-ая часть ответа
                    PowerProfileRecord powerProfileRecord1 = new PowerProfileRecord();

                    string date = $"{Convert.ToInt32(_readingResponse.Data[4]).ToString("X")}.{Convert.ToInt32(_readingResponse.Data[5]).ToString("X")}.{Convert.ToInt32(_readingResponse.Data[6]).ToString("X")} {Convert.ToInt32(_readingResponse.Data[2]).ToString("X")}:{Convert.ToInt32(_readingResponse.Data[3]).ToString("X")}";

                    if (!DateTime.TryParse(date, out _dateTimeOfRecord))
                        DateTime.TryParse("12.12.12", out _dateTimeOfRecord);
                    powerProfileRecord1.RecordDate = _dateTimeOfRecord;

                    powerProfileRecord1.Pplus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[8],
                        older: _readingResponse.Data[9],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    powerProfileRecord1.Pminus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[10],
                        older: _readingResponse.Data[11],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    powerProfileRecord1.Qplus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[12],
                        older: _readingResponse.Data[13],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    powerProfileRecord1.Qminus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[14],
                        older: _readingResponse.Data[15],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    _response.Records.Enqueue(powerProfileRecord1);
                    /*                    _response.Logs.Enqueue(new Logs()
                                        {
                                            Date = DateTime.Now,
                                            Status = CommonVariables.SUCCESS_LOG_STATUS,
                                            Description = $"Power profile for {_dateTimeOfRecord} was successfully read"
                                        });
                    */
                    //2-ая часть ответа
                    PowerProfileRecord powerProfileRecord2 = new PowerProfileRecord();

                    date = $"{Convert.ToInt32(_readingResponse.Data[19]).ToString("X")}.{Convert.ToInt32(_readingResponse.Data[20]).ToString("X")}.{Convert.ToInt32(_readingResponse.Data[21]).ToString("X")} {Convert.ToInt32(_readingResponse.Data[17]).ToString("X")}:{Convert.ToInt32(_readingResponse.Data[18]).ToString("X")}";

                    if (!DateTime.TryParse(date, out _dateTimeOfRecord))
                        DateTime.TryParse("12.12.12", out _dateTimeOfRecord);
                    powerProfileRecord2.RecordDate = _dateTimeOfRecord;

                    powerProfileRecord2.Pplus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[23],
                        older: _readingResponse.Data[24],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    powerProfileRecord2.Pminus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[25],
                        older: _readingResponse.Data[26],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    powerProfileRecord2.Qplus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[27],
                        older: _readingResponse.Data[28],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    powerProfileRecord2.Qminus = CommunicCommon.PowerProfileBytesHandling(young: _readingResponse.Data[29],
                        older: _readingResponse.Data[30],
                        T: _METER_T,
                        A: _METER_CONSTANT);
                    _response.Records.Enqueue(powerProfileRecord2);
                    /* _response.Logs.Enqueue(new Logs()
                     {
                         Date = DateTime.Now,
                         Status = CommonVariables.SUCCESS_LOG_STATUS,
                         Description = $"Power profile for {_dateTimeOfRecord} was successfully read"
                     });*/

                    CommunicCommon.ComputeNextOlderYoungBytes(young: ref _YOUNG_BYTE,
                        older: ref _OLDER_BYTE,
                        bit17for234: ref _bit17for234,
                        powerProfileRequestCode: ref powerProfileRequestCode);

                    return _response;
                }
            }

            _response.Exception = $"Error: No response on GetPowerProfileRecord request (address: {_address})";
            return _response;
        }
        internal async Task<EnergyRecordResponse> GetEnergyValuesLocalAsync(int month_number, int year_number)
        {
            if (!CommunicCommon.CheckEnergyMonthYear(month_number, year_number))
                throw new Exception("Error: incorrect month or year (GetEnergyValues)");

            EnergyRecordResponse _response = new EnergyRecordResponse();
            _response.MeterAddress = _address;
            _response.MonthNumber = month_number;
            _response.Year = year_number;
            _response.Logs.Enqueue(new Logs()
            {
                Date = DateTime.Now,
                Status = CommonVariables.INFO_LOG_STATUS,
                Description = $"GetEnergyValues request (address:{_address} month:{_response.MonthNumber} year:{_response.Year})"
            });

            string b = "B";
            string b2 = "B";
            int nextMonth = 0;

            switch (_response.MonthNumber)
            {
                case 10:
                    {
                        b = b + "A";
                        nextMonth = 11;
                        b2 = b2 + "B";
                        break;
                    }
                case 11:
                    {
                        b = b + "B";
                        nextMonth = 12;
                        b2 = b2 + "C";
                        break;
                    }
                case 12:
                    {
                        b = b + "C";
                        nextMonth = 1;
                        b2 = b2 + "1";
                        break;
                    }
                default:
                    {
                        b = b + _response.MonthNumber.ToString();
                        nextMonth = _response.MonthNumber + 1;
                        b2 = b2 + Convert.ToString(nextMonth);
                        break;
                    }
            }

            byte[] request_crc = new byte[4] { _hexAddress, 0x05, Convert.ToByte(b, 16), 0x00 };
            byte[] crc = _CRCCalc.CalculateCrc(request_crc, _CrcCalcAlgorithm);
            byte[] request = new byte[6] { _hexAddress, 0x05, Convert.ToByte(b, 16), 0x00, crc[0], crc[1] };
            byte[] request_crc_end = request_crc;
            request_crc_end[2] = Convert.ToByte(b2, 16);
            crc = _CRCCalc.CalculateCrc(request_crc_end, _CrcCalcAlgorithm);

            try
            {
                EnergyCommunicResponse _startValueReadingResponse = await GetEnergyCommunicResponseAsync(message: request,
                    monthNumber: _response.MonthNumber,
                    year: _response.Year);

                _response.StartValue = _startValueReadingResponse.Value;
                _response.Logs = DevicesCommon.JoinTwoQueuesHook(_startValueReadingResponse.Logs, _response.Logs) ?? _response.Logs;

                request_crc[2] = Convert.ToByte(b2, 16);
                crc = _CRCCalc.CalculateCrc(request_crc, _CrcCalcAlgorithm);
                request[2] = request_crc[2];
                request[4] = crc[0];
                request[5] = crc[1];

                EnergyCommunicResponse _endValueReadingResponse = await GetEnergyCommunicResponseAsync(message: request,
                    monthNumber: _response.MonthNumber,
                    year: _response.Year);

                _response.EndValue = _endValueReadingResponse.Value;
                _response.Logs = DevicesCommon.JoinTwoQueuesHook(_endValueReadingResponse.Logs, _response.Logs) ?? _response.Logs;

                _response.TotalValue = _response.EndValue - _response.StartValue;
                return _response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task<EnergyCommunicResponse> GetEnergyCommunicResponseAsync(byte[] message, int monthNumber, int year)
        {
            EnergyCommunicResponse _response = new EnergyCommunicResponse();

            for (int i = 0; i < CommonVariables.REPEAT_REQUESTS_COUNT; i++)
            {
                _communication.WriteMessageGSM(_serialPort, message);
                GSMReadingResponse _readingResponse = await _communication.ReadGSMResponseAsync(serialPort: _serialPort,
                    responseSize: 19);

                if (_readingResponse.TimeOverFlag)
                {
                    _response.Logs.Enqueue(new Logs()
                    {
                        Date = DateTime.Now,
                        Status = CommonVariables.WARNING_LOG_STATUS,
                        Description = $"No response on GetEnergyValues request (address: {_address} month:{monthNumber} year:{year})"
                    });
                }
                else
                {
                    byte[] check_crc_massa = _readingResponse.Data.Take(17).ToArray();
                    byte[] crc_out = _CRCCalc.CalculateCrc(check_crc_massa, _CrcCalcAlgorithm);

                    if (crc_out[0] != _readingResponse.Data[17] || crc_out[1] != _readingResponse.Data[18])
                        continue;

                    _response.Value = EnergyValuesHandling(_readingResponse.Data);
                    return _response;
                }
            }
            throw new Exception($"Error: No response on GetEnergyValues request (address: {_address} month:{monthNumber} year:{year})");
        }
        private double EnergyValuesHandling(List<byte> data)
        {
            List<string> energyBytesList0 = new List<string>() { data[1].ToString("X"), data[2].ToString("X"),
                        data[3].ToString("X"), data[4].ToString("X") };
            List<string> energyBytesList = new List<string>();

            foreach (string energyByte in energyBytesList0)
            {
                string energyByteCopy = energyByte;
                if ((from t in CommonVariables.BAG_LIST
                     where t == energyByteCopy
                     select t).Any())
                {
                    energyByteCopy = energyByteCopy.Insert(0, "0");
                }
                energyBytesList.Add(energyByteCopy);
            }
            string energyBytesStr = energyBytesList[1] + energyBytesList[0] + energyBytesList[3] + energyBytesList[2];
            long energuBytesInt = Convert.ToInt64(energyBytesStr, 16);
            double energyBytesFl = Convert.ToSingle(energuBytesInt) / 1000;

            return energyBytesFl;
        }
    }
}
