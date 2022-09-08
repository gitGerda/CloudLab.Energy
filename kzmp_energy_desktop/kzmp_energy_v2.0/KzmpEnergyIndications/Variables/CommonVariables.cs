using KzmpEnergyIndicationsLibrary.Models.Meter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Variables
{
    public static class CommonVariables
    {
        internal static readonly List<byte> CORRECTIVE_LIST_FOR_YOUNG_BYTE = new List<byte>() { 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0, 0xf0 };

        internal static readonly List<string> BAG_LIST = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

        internal static readonly List<float> METERS_CONSTANTS = new List<float>() { 5000, 25000, 1250, 500, 1000, 250 };

        internal const int REPEAT_REQUESTS_COUNT = 3;

        public const int REPEAT_CONNECTION_ATTEMPTS_COUNT = 5;
        public const int REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC = 60000; //5minutes(300000)

        public const string ERROR_LOG_STATUS = "error";
        public const string WARNING_LOG_STATUS = "warning";
        public const string INFO_LOG_STATUS = "info";
        public const string SUCCESS_LOG_STATUS = "success";

        //Поддериживаемые модели счётчиков
        //**Примечание: типы модели счётчиков регистрозависимы
        public static readonly List<MeterType> METERS_TYPES = new List<MeterType>()
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
        //Словарь поддерживаемых интерфейсов связи
        public static readonly List<string> COMMUNIC_INTERFACES = new List<string>()
        {
            "GSM",
            "GSM-шлюз"
        };
    }
}
