using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KzmpEnergyIndicationsLibrary.Models.Meter;
using KzmpEnergyIndicationsLibrary.Variables;

namespace KzmpEnergyIndicationsLibrary.Actions.Communicating
{
    public static class CommunicCommon
    {
        public static int ComputeHalfHoursCount(DateTime startDate, DateTime endDate)
        {
            var countVar = (endDate - startDate).Duration();
            int count = Convert.ToInt32(countVar.Days) * 24 * 2 + Convert.ToInt32(countVar.Hours) * 2 + Convert.ToInt32(countVar.Minutes) / 30;
            return count;
        }
        internal static float PowerProfileBytesHandling(byte young, byte older, float T, float A)
        {
            string valueYoung = young.ToString("X");
            if ((from t in CommonVariables.BAG_LIST
                 where t == valueYoung
                 select t).Any())
            {
                valueYoung = valueYoung.Insert(0, "0");
            }
            string valueStr = older.ToString("X") + valueYoung;
            int valueInt32 = Convert.ToInt32(valueStr, 16);
            float valueFloat = Convert.ToSingle(valueInt32);
            valueFloat = valueFloat * 60 / T / (2 * A);

            if (valueStr == "FFFF")
                valueFloat = 0;

            return valueFloat;
        }
        internal static void ComputeNextOlderYoungBytes(ref byte young, ref byte older, ref byte bit17for234, ref byte powerProfileRequestCode)
        {
            if (young != 0xE0 & young != 0xF0)
            {
                young = Convert.ToByte(young + 0x20);
            }
            else
            {
                if (older != 0xFF)
                {
                    older = Convert.ToByte(older + 0x01);
                }
                else
                {
                    older = 0x00;
                    //смена 17-го бита для Меркурия 234 происходит при переходе старшего байта с 0x00 в 0xff и наоборот
                    if (bit17for234 == 0x03)
                    {
                        bit17for234 = 0x83;
                    }
                    else
                    {
                        bit17for234 = 0x03;
                    }
                    powerProfileRequestCode = bit17for234;
                }
                young = 0x00;
            }
        }
        internal static void ComputeOlderYoungBytesForStartDate(List<byte> _LAST_PARAMETR, IMeterType _meterType, DateTime _startDate, ref byte _YOUNG_BYTE, ref byte _OLDER_BYTE, ref byte _bit17for234)
        {
            _OLDER_BYTE = _LAST_PARAMETR[1];
            _YOUNG_BYTE = _LAST_PARAMETR[2];

            if (_meterType.Name == "Меркурий" && _meterType.Type == "234")
            {
                string BagOlder = _OLDER_BYTE.ToString("X");
                string BagYoung = _YOUNG_BYTE.ToString("X");
                string k = "";

                if ((from t in CommonVariables.BAG_LIST
                     where t == BagOlder
                     select t).Any())
                {
                    BagOlder = BagOlder.Insert(0, "0");
                }

                if ((from t in CommonVariables.BAG_LIST
                     where t == BagYoung
                     select t).Any())
                {
                    BagYoung = BagYoung.Insert(0, "0");
                }

                k = BagOlder + BagYoung + "0";

                string localOlder = Convert.ToString(k[1]) + Convert.ToString(k[2]);
                string localYoung = Convert.ToString(k[3]) + Convert.ToString(k[4]);

                int z = Convert.ToInt32(localOlder, 16);
                int s = Convert.ToInt32(localYoung, 16);

                _OLDER_BYTE = Convert.ToByte(z);
                _YOUNG_BYTE = Convert.ToByte(s);
            }

            byte hour = _LAST_PARAMETR[4];
            int hour_b = Convert.ToInt32(hour);

            byte minute = _LAST_PARAMETR[5];
            int minute_b = Convert.ToInt32(minute);

            byte day = _LAST_PARAMETR[6];
            int day_b = Convert.ToInt32(day);

            byte month = _LAST_PARAMETR[7];
            int month_b = Convert.ToInt32(month);

            byte year = _LAST_PARAMETR[8];
            int year_b = Convert.ToInt32(year);

            string last_date_str = day_b.ToString("X") + "." + month_b.ToString("X") + "." + year_b.ToString("X") + " " + hour_b.ToString("X") + ":" + minute_b.ToString("X");
            DateTime last_date = DateTime.Parse(last_date_str);

            int count = ComputeHalfHoursCount(startDate: _startDate, endDate: last_date);

            for (int i = count; i >= 0; i--)
            {
                if (_YOUNG_BYTE == 0x00)
                {
                    if (_OLDER_BYTE != 0x00)
                    {
                        _OLDER_BYTE = Convert.ToByte(_OLDER_BYTE - 0x01);
                    }
                    else
                    {
                        _OLDER_BYTE = 0xff;
                        //смена 17-го бита для Меркурия 234 происходит при переходе старшего байта с 0x00 в 0xff и наоборот
                        if (_bit17for234 == 0x03)
                        {
                            _bit17for234 = 0x83;
                        }
                        else
                        {
                            _bit17for234 = 0x03;
                        }
                    }
                    _YOUNG_BYTE = 0xf0;
                }
                else
                {
                    _YOUNG_BYTE = Convert.ToByte(_YOUNG_BYTE - 0x10);
                }
            }

            byte _YOUNG_BYTE_COPY = _YOUNG_BYTE;
            if ((from t in CommonVariables.CORRECTIVE_LIST_FOR_YOUNG_BYTE
                 where t == _YOUNG_BYTE_COPY
                 select t).Any())
            {
                _YOUNG_BYTE = Convert.ToByte(_YOUNG_BYTE - 0x10);
            }
        }
        internal static bool CheckEnergyMonthYear(int _energyMonthNumber, int _energyYear)
        {
            if (_energyMonthNumber < 1 || _energyMonthNumber > 12 || DateTime.Now.Year < _energyYear)
                return false;

            if (DateTime.Now.Year - _energyYear == 0)
            {
                if (_energyMonthNumber > DateTime.Now.Month - 1)
                {
                    return false;
                }
            }
            else if (DateTime.Now.Year - _energyYear == 1)
            {
                if (_energyMonthNumber < DateTime.Now.Month)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
