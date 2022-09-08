using kzmpCloudAPI.Controllers.IndicationsControllers.DatabasePart;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.ControlersTest.IndicationsControllersTest.PowerIndicationControllerTest
{
    public class PowerIndicationsControllerTest : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture _database_fixture
        {
            get;
        }
        public PowerIndicationsControllerTest(KzmpEnergyTestingDbFixture database_fixture)
        {
            _database_fixture = database_fixture;
        }
        //-------------------------------------------------------------------------------
        //DeleteIndications
        [Theory]
        [InlineData("incorrect", "01.01.2001")]
        [InlineData("01.01.2001", "incorrect")]
        public void DeleteIndications_OnIncorrectStartOrEndDate_BadRequestResult(string start_date, string end_date)
        {
            //-------------------------------
            var _logger = new Mock<ILogger<PowerIndicationsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _target = new PowerIndicationsController(_database, _logger);
            //------------------------------
            var _result = _target.DeleteIndications(1, start_date, end_date);
            //------------------------------
            Assert.IsType<BadRequestResult>(_result);
        }
        [Theory]
        [InlineData(8365, "12.05.22", "18.08.22")]
        public void DeleteIndications_OnInvoke_ZeroRecordsWithSameParameters(int address, string start_date, string end_date)
        {
            //------------------------------
            _create_power_profile_records(address.ToString(), start_date, end_date);
            var _logger = new Mock<ILogger<PowerIndicationsController>>().Object;
            var _database = _database_fixture.CreateContext();
            Assert.True(_database.PowerProfileMs.Where(t => t.Address == address && t.Date >= DateTime.Parse(start_date) && t.Date <= DateTime.Parse(end_date)).Any());
            var _target = new PowerIndicationsController(_database, _logger);
            //------------------------------
            var _result = _target.DeleteIndications(address, start_date, end_date);
            //------------------------------
            var _database2 = _database_fixture.CreateContext();
            Assert.IsType<OkResult>(_result);
            Assert.False(_database2.PowerProfileMs.Where(t => t.Address == address && t.Date >= DateTime.Parse(start_date) && t.Date <= DateTime.Parse(end_date)).Any());
        }

        private int _create_power_profile_records(string meter_address, string start_date, string end_date)
        {
            var _database = _database_fixture.CreateContext();
            var _max_row_number = _database.PowerProfileMs.Max(t => t.RowNumber);

            DateTime _start = DateTime.Parse(start_date);
            DateTime _end = DateTime.Parse(end_date);
            int _address = Convert.ToInt32(meter_address);

            while (DateTime.Compare(_start, _end) < 0)
            {
                var _record = new PowerProfileM()
                {
                    Address = _address,
                    Date = _start,
                    Id = 1,
                    Pminus = 0,
                    Pplus = 0,
                    Qminus = 0,
                    Qplus = 0,
                    RowNumber = _max_row_number,
                    Time = TimeSpan.Zero
                };
                _max_row_number++;
                _start = _start.AddDays(1);
                _database.PowerProfileMs.Add(_record);
            }
            return _database.SaveChanges();
        }

    }
}
