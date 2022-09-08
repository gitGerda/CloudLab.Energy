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
    public class GetEnergyIndicByAddrStartEnd : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture Fixture
        {
            get;
        }
        public GetEnergyIndicByAddrStartEnd(KzmpEnergyTestingDbFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetIndicByAddrStartEnd_GetCountAllAddressesAllDate()
        {
            //Arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);
            int generalCountOfRows = KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES.Length * KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER;

            //Act
            var result = controller.GetEnergyIndicByAddrStartEnd(
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                endDate.ToString(),
                null, null,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, true) as JsonResult;

            //Assert
            Assert.Equal(generalCountOfRows.ToString(), result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page1_LastRowAddress()
        {
            //Arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //Act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrStartEnd(
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                endDate.ToString(),
                1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0].ToString(),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }
        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page1_count()
        {
            //Arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //Act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrStartEnd(
                 KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                 endDate.ToString(),
                 1,
                 KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                 KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                 false))?.Value as List<EnergyTable>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), result?.Count().ToString());
        }

        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page2_LastRowAddress()
        {
            //Arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //Act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrStartEnd(
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                endDate.ToString(),
                2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1].ToString(),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }
        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page2_count()
        {
            //Arrange
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            DateTime endDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            //Act
            var result = ((JsonResult)controller.GetEnergyIndicByAddrStartEnd(
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                endDate.ToString(),
                2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false))?.Value as List<EnergyTable>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), result?.Count().ToString());
        }
    }
}
