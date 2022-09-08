using kzmpCloudAPI.Controllers.IndicationsControllers.DatabasePart;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Tests.Fixtures;
using kzmpCloudAPI.Tests.Hooks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace kzmpCloudAPI.Tests.ControlersTest.IndicationsControllersTest.EnergyIndicationsControllerTest
{
    public class GetEnergyIndicByAddrEnd : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture Fixture
        {
            get;
        }
        public GetEnergyIndicByAddrEnd(KzmpEnergyTestingDbFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetRecords_NullAddresses()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            //act
            var result = controller.GetEnergyIndicByAddrEnd(null, null, null, null, false);

            //assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetCount_RecordsCountForAllAddresses()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            string RowsCount = Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES.Length * KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = controller.GetEnergyIndicByAddrEnd(endDate.ToString(),
                null, null, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, true) as JsonResult;

            //assert
            Assert.Equal(RowsCount, result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetRecords_Page1Count()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddYears
                (KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrEnd(endDate.ToString(),
                1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER), result?.Count().ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetRecords_Page2Count()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrEnd(
                endDate.ToString(),
                2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false
                ))?.Value as List<EnergyTable>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER), result?.Count().ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetRecords_Page1LastRecordAddress()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrEnd(endDate.ToString(),
                1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0]),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetRecords_Page2LastRecordAddress()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrEnd(endDate.ToString(),
                2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1]),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddrEnd_GetRecords_Page2FirstRecordAddress()
        {
            //arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = Convert.ToDateTime(KzmpEnergyTestingDbFixture.DEFAULT_START_DATE).AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrEnd(endDate.ToString(),
                2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //assert
            Assert.Equal(Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1]),
                result?[0].Address.ToString());
        }
    }
}
