using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using kzmpCloudAPI.Controllers.IndicationsControllers.DatabasePart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.EntityFrameworkCore;
using kzmpCloudAPI.Tests.Hooks;
using kzmpCloudAPI.Tests.Fixtures;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Tests.ControlersTest.IndicationsControllersTest.PowerIndicationControllerTest
{
    public class GetIndicByAddrStartEnd : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture Fixture
        {
            get;
        }
        public GetIndicByAddrStartEnd(KzmpEnergyTestingDbFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetIndicByAddrStartEnd_GetCountAllAddressesAllDate()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new PowerIndicationsController(context);

            //Act
            var result = controller.GetIndicByAddrStartEnd(null, null, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(), "01.01.2023", true) as JsonResult;

            //Assert
            Assert.Equal("500", result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetIndicByAddrStartEnd_GetCountFromAddr1ByMonth31day()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new PowerIndicationsController(context);
            int[] Addresses = new int[] { KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0] };
            DateTime nextDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddDays(31);

            //Act
            var result = controller.GetIndicByAddrStartEnd(null, null, Addresses, KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(), nextDate.ToString(), true) as JsonResult;

            //Assert
            Assert.Equal("31", result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page1_LastRowAddress()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new PowerIndicationsController(context);
            DateTime nextDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(10);

            //Act
            List<PowerProfileM>? result = ((JsonResult)controller.GetIndicByAddrStartEnd(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                nextDate.ToString(), false))?.Value as List<PowerProfileM>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0].ToString(),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }
        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page1_count()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new PowerIndicationsController(context);
            DateTime nextDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(10);

            //Act
            List<PowerProfileM>? result = ((JsonResult)controller.GetIndicByAddrStartEnd(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                nextDate.ToString(), false))?.Value as List<PowerProfileM>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), result?.Count().ToString());
        }

        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page2_LastRowAddress()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new PowerIndicationsController(context);
            DateTime nextDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(10);

            //Act
            List<PowerProfileM>? result = ((JsonResult)controller.GetIndicByAddrStartEnd(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                nextDate.ToString(), false))?.Value as List<PowerProfileM>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1].ToString(),
                result?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }
        [Fact]
        public void GetIndicByAddrStartEnd_Pagination_Page2_count()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new PowerIndicationsController(context);
            DateTime nextDate = KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.AddYears(10);

            //Act
            List<PowerProfileM>? result = ((JsonResult)controller.GetIndicByAddrStartEnd(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                KzmpEnergyTestingDbFixture.DEFAULT_START_DATE.ToString(),
                nextDate.ToString(), false))?.Value as List<PowerProfileM>;

            //Assert
            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), result?.Count().ToString());
        }
    }
}
