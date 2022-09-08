using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using KzmpEnergyIndicationsLibrary.Variables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace kzmpCloudAPI.Tests.Fixtures
{
    public class KzmpEnergyTestingDbFixture
    {
        //const string connectionStr = "Server = WS-OIT-007\\SQLEXPRESS;Database = kzmp_energy_Test; User Id = TestUser; Password = qwerty852456;";
        //const string connectionStr = "Server = MEMENTO_MORI\\SQLEXPRESS;Database = kzmp_energy_Test; User Id = TestUser; Password = 852456;";
        //const string connectionStr = "Server=192.168.43.3;port=3307;user=root;pwd=Otohof96;Database=kzmp_energy_test";
        private readonly string connectionStr;
        public static readonly int[] DEFAULT_ADDRESSES = new int[] { 1, 2, 3, 4, 5 };
        public static readonly DateTime DEFAULT_START_DATE = Convert.ToDateTime("01.01.2022");

        //POWER_PROFILE_MS
        public static readonly int DEFAULT_ROWS_FOR_METER = 100;

        //ENERGY_TABLE
        public static readonly Dictionary<int, string> MONTH_DICTIONARY = new Dictionary<int, string>() {
            {1,"Январь"},
            {2,"Февраль"},
            {3,"Март"},
            {4,"Апрель"},
            {5,"Май"},
            {6,"Июнь"},
            {7,"Июль"},
            {8,"Август"},
            {9,"Сентябрь"},
            {10,"Октябрь"},
            {11,"Ноябрь"},
            {12,"Декабрь"},
        };

        static readonly object _lock = new();
        static bool _databaseInitialized = false;


        public KzmpEnergyTestingDbFixture()
        {
            lock (_lock)
            {
                string machine_name = Environment.MachineName.ToLower();
                if (machine_name == "memento_mori")
                {
                    connectionStr = "Server=192.168.43.3;port=3307;user=root;pwd=Otohof96;Database=kzmp_energy_test";
                }
                else if (machine_name == "ws-oit-007")
                {
                    connectionStr = "Server=192.168.0.64;port=3307;user=root;pwd=Otohof96;Database=kzmp_energy_test";
                }
                else
                {
                    connectionStr = "";
                }

                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.PowerProfileMs.AddRange(createTestPowerProfileMs());
                        context.EnergyTables.AddRange(createTestEnergyTables());
                        context.Meters.AddRange(CreateTestMeterTable());
                        context.MsgNumbers.AddRange(CreateMsgNumbersRecords());
                        context.ShedulesMeters.Add(new ShedulesMeter()
                        {
                            MeterId = 1,
                            SheduleId = 1
                        });
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        MsgNumber[] CreateMsgNumbersRecords()
        {
            List<MsgNumber> _msg_numbers = new List<MsgNumber>();
            foreach (var _address in DEFAULT_ADDRESSES)
            {
                var _msg_number = new MsgNumber()
                {
                    CompanyInn = "test_inn#" + _msg_numbers.Count,
                    CompanyName = "test_company#" + _msg_numbers.Count
                };
                _msg_numbers.Add(_msg_number);
            }

            _msg_numbers.Add(new MsgNumber()
            {
                CompanyInn = "12345678",
                CompanyName = "КЗМП",
                Contract = "12345",
                Date = "01.02.2022",
                MsgNumber1 = "12"
            });
            _msg_numbers.Add(new MsgNumber()
            {
                CompanyInn = "90872367164",
                CompanyName = "Расплав",
                Contract = "67890",
                Date = "14.09.2001",
                MsgNumber1 = "400"
            });
            return _msg_numbers.ToArray();
        }
        Meter[] CreateTestMeterTable()
        {
            var _random = new Random();
            List<Meter> _meters_table = new List<Meter>();
            foreach (var _address in DEFAULT_ADDRESSES)
            {
                Meter _meter = new Meter()
                {
                    /*                    IdMeter = _meters_table.Count,
                    */
                    Address = _address,
                    CenterName = "test_center_name",
                    CompanyName = "test_company#" + _meters_table.Count,
                    Inn = "test_inn#" + _meters_table.Count,
                    Inncenter = "test_center_inn#" + _meters_table.Count,
                    Interface = CommonVariables.COMMUNIC_INTERFACES[_random.Next(0, 2)],
                    MeasuringchannelA = "",
                    MeasuringchannelR = "",
                    MeasuringpointName = "",
                    Sim = "test_sim#" + _meters_table.Count,
                    TransformationRatio = "",
                    Type = "",
                    Xml80020code = ""
                };
                _meters_table.Add(_meter);
            }
            return _meters_table.ToArray();
        }

        EnergyTable[] createTestEnergyTables()
        {
            int[] defaultAddresses = DEFAULT_ADDRESSES;
            DateTime defaultStartDate = DEFAULT_START_DATE;
            EnergyTable[] energyTables = new EnergyTable[DEFAULT_ROWS_FOR_METER * DEFAULT_ADDRESSES.Length];

            for (int i = 0; i < DEFAULT_ROWS_FOR_METER * DEFAULT_ADDRESSES.Length; i++)
            {
                if (i < DEFAULT_ROWS_FOR_METER)
                {
                    AddEnergyTableRow(ref energyTables, i, ref defaultStartDate, defaultAddresses[0]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER && i < DEFAULT_ROWS_FOR_METER * 2)
                {
                    if (i == DEFAULT_ROWS_FOR_METER)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddEnergyTableRow(ref energyTables, i, ref defaultStartDate, defaultAddresses[1]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER * 2 && i < DEFAULT_ROWS_FOR_METER * 3)
                {
                    if (i == DEFAULT_ROWS_FOR_METER * 2)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddEnergyTableRow(ref energyTables, i, ref defaultStartDate, defaultAddresses[2]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER * 3 && i < DEFAULT_ROWS_FOR_METER * 4)
                {
                    if (i == DEFAULT_ROWS_FOR_METER * 3)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddEnergyTableRow(ref energyTables, i, ref defaultStartDate, defaultAddresses[3]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER * 4 && i <= DEFAULT_ROWS_FOR_METER * 5)
                {
                    if (i == DEFAULT_ROWS_FOR_METER * 4)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddEnergyTableRow(ref energyTables, i, ref defaultStartDate, defaultAddresses[4]);
                }
            }
            return energyTables;
        }
        PowerProfileM[] createTestPowerProfileMs()
        {
            int[] defaultAddresses = DEFAULT_ADDRESSES;
            DateTime defaultStartDate = DEFAULT_START_DATE;
            PowerProfileM[] powerProfileMs = new PowerProfileM[DEFAULT_ROWS_FOR_METER * DEFAULT_ADDRESSES.Length];

            for (int i = 0; i < DEFAULT_ROWS_FOR_METER * DEFAULT_ADDRESSES.Length; i++)
            {
                if (i < DEFAULT_ROWS_FOR_METER)
                {
                    AddPowerMsRow(ref powerProfileMs, i, ref defaultStartDate, defaultAddresses[0]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER && i < DEFAULT_ROWS_FOR_METER * 2)
                {
                    if (i == DEFAULT_ROWS_FOR_METER)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddPowerMsRow(ref powerProfileMs, i, ref defaultStartDate, defaultAddresses[1]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER * 2 && i < DEFAULT_ROWS_FOR_METER * 3)
                {
                    if (i == DEFAULT_ROWS_FOR_METER * 2)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddPowerMsRow(ref powerProfileMs, i, ref defaultStartDate, defaultAddresses[2]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER * 3 && i < DEFAULT_ROWS_FOR_METER * 4)
                {
                    if (i == DEFAULT_ROWS_FOR_METER * 3)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddPowerMsRow(ref powerProfileMs, i, ref defaultStartDate, defaultAddresses[3]);
                }
                else if (i >= DEFAULT_ROWS_FOR_METER * 4 && i <= DEFAULT_ROWS_FOR_METER * 5)
                {
                    if (i == DEFAULT_ROWS_FOR_METER * 4)
                    {
                        defaultStartDate = DEFAULT_START_DATE;
                    }
                    AddPowerMsRow(ref powerProfileMs, i, ref defaultStartDate, defaultAddresses[4]);
                }
            }
            return powerProfileMs;
        }

        void AddPowerMsRow(ref PowerProfileM[] ms, int i, ref DateTime defaultStartDate, int address)
        {
            defaultStartDate = defaultStartDate.AddDays(1);
            PowerProfileM local = new PowerProfileM()
            {
                Id = address,
                Address = address,
                Date = defaultStartDate,
                Time = TimeSpan.Zero
            };
            ms[i] = local;
        }

        void AddEnergyTableRow(ref EnergyTable[] ms, int i, ref DateTime defaultStartDate, int address)
        {
            defaultStartDate = defaultStartDate.AddMonths(1);
            EnergyTable local = new EnergyTable()
            {
                MeterId = address,
                Address = address,
                Year = defaultStartDate.Year,
                //Month = MONTH_DICTIONARY[defaultStartDate.Month]
                Month = defaultStartDate.Month.ToString()
            };
            ms[i] = local;
        }
        public kzmp_energyContext CreateContext()
        {
            /* return new kzmp_energyContext(new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<kzmp_energyContext>().
                  UseSqlServer(connectionStr).Options);*/
            return new kzmp_energyContext(new DbContextOptionsBuilder<kzmp_energyContext>().UseMySql(connectionStr, ServerVersion.Parse("8.0.29-mysql")).Options);
        }
    }
}
