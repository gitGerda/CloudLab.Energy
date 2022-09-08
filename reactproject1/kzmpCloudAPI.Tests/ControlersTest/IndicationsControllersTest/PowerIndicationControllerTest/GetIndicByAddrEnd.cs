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
    public class GetIndicByAddrEnd : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture Fixture
        {
            get;
        }
        public GetIndicByAddrEnd(KzmpEnergyTestingDbFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetIndicByAddrEnd_GetRecords_NullAddresses()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());

            //act
            var result = controller.GetIndicByAddrStart(null, null, null, null, false);

            //assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void GetIndicByAddrEnd_GetCount_RecordsCountForAllAddresses()
        {
            //arrange
            var _database = Fixture.CreateContext();
            PowerIndicationsController controller = new PowerIndicationsController(_database);
            /*            string RowsCount = Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES.Length * KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);
            */
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddDays(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);
            string RowsCount = _database.PowerProfileMs.Select(t => t).Count().ToString();

            //act
            var result = controller.GetIndicByAddrEnd(null,
                null,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                Convert.ToString(endDate),
                true) as JsonResult;

            //assert
            Assert.Equal(RowsCount, result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetIndicByAddrEnd_GetRecords_Page1Count()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddDays(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetIndicByAddrEnd(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                Convert.ToString(endDate),
                false))?.Value as List<PowerProfileM>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER), result?.Count().ToString());
        }

        [Fact]
        public void GetIndicByAddrEnd_GetRecords_Page2Count()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddDays(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetIndicByAddrEnd(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                Convert.ToString(endDate),
                false))?.Value as List<PowerProfileM>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER), result?.Count().ToString());
        }

        [Fact]
        public void GetIndicByAddrEnd_GetRecords_Page1LastRecordAddress()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddDays(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetIndicByAddrEnd(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                Convert.ToString(endDate),
                false))?.Value as List<PowerProfileM>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0]),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }

        [Fact]
        public void GetIndicByAddrEnd_GetRecords_Page2LastRecordAddress()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddDays(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetIndicByAddrEnd(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                Convert.ToString(endDate),
                false))?.Value as List<PowerProfileM>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1]),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }

        [Fact]
        public void GetIndicByAddrEnd_GetRecords_Page2FirstRecordAddress()
        {
            //arrange
            PowerIndicationsController controller = new PowerIndicationsController(Fixture.CreateContext());
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddDays(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetIndicByAddrEnd(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                Convert.ToString(endDate),
                false))?.Value as List<PowerProfileM>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1]),
                result?[0].Address.ToString());
        }
    }
}
