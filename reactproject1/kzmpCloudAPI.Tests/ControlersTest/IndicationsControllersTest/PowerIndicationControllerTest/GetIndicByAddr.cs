using kzmpCloudAPI.Controllers.IndicationsControllers.DatabasePart;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Tests.Fixtures;
using kzmpCloudAPI.Tests.Hooks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace kzmpCloudAPI.Tests.ControlersTest.IndicationsControllersTest.PowerIndicationControllerTest
{
    public class GetIndicByAddr : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture Fixture
        {
            get;
        }
        public GetIndicByAddr(KzmpEnergyTestingDbFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetIndicByAddr_GetRecordsCount_AddrNull()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddr(null, null, null, true);

            //assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void GetIndicByAddr_GetRecordsCount_DefaultAddresses()
        {
            //arrange
            var _database = Fixture.CreateContext();
            PowerIndicationsController controller = new PowerIndicationsController(_database);
            /*            string countOfRows = Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES.Length * KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);
            */
            string countOfRows = _database.PowerProfileMs.Select(t => t).Where(t => t.Address == 1 || t.Address == 2 || t.Address == 3 || t.Address == 4 || t.Address == 5).Count().ToString();
            //act
            var result = controller.GetIndicByAddr(null, null, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, true) as JsonResult;

            //assert
            Assert.Equal(countOfRows, result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetIndicByAddr_GetRecordsPage1_CheckCount()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddr(1, KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, false) as JsonResult;
            List<PowerProfileM>? resultGen = result?.Value as List<PowerProfileM>;

            //assert
            Assert.NotNull(resultGen);
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), resultGen?.Count().ToString());
        }

        [Fact]
        public void GetIndicByAddr_GetRecordsPage1_CheckLastRecordAddr()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddr(1, KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, false) as JsonResult;
            List<PowerProfileM>? resultGen = result?.Value as List<PowerProfileM>;

            //assert
            Assert.NotNull(resultGen);
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0].ToString(), resultGen?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1]?.Address.ToString());
        }
        [Fact]
        public void GetIndicByAddr_GetRecordsPage2_CheckCount()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddr(2, KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, false) as JsonResult;
            List<PowerProfileM>? resultGen = result?.Value as List<PowerProfileM>;

            //assert
            Assert.NotNull(resultGen);
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), resultGen?.Count().ToString());
        }
        [Fact]
        public void GetIndicByAddr_GetRecordsPage2_CheckLastRecordAddr()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddr(2, KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, false) as JsonResult;
            List<PowerProfileM>? resultGen = result?.Value as List<PowerProfileM>;

            //assert
            Assert.NotNull(resultGen);
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1].ToString(), resultGen?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1]?.Address.ToString());
        }

        [Fact]
        public void GetIndicByAddr_GetRecordsPage2_CheckFirstRecordAddr()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddr(2, KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, false) as JsonResult;
            List<PowerProfileM>? resultGen = result?.Value as List<PowerProfileM>;

            //assert
            Assert.NotNull(resultGen);
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1].ToString(), resultGen?[0]?.Address.ToString());
        }
    }
}
