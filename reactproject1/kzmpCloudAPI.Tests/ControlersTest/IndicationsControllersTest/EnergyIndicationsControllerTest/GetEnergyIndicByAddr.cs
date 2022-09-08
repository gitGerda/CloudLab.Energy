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
    public class GetEnergyIndicByAddr : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture Fixture
        {
            get;
        }
        public GetEnergyIndicByAddr(KzmpEnergyTestingDbFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsCount_AddrNull()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            var result = controller.GetEnergyIndicByAddr(null, null, null, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsCount_DefaultAddresses()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);
            string countOfRows = Convert.ToString(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES.Length * KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER);

            var result = controller.GetEnergyIndicByAddr(null, null, KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES, true) as JsonResult;

            Assert.Equal(countOfRows, result?.Value?.GetReflectedProperty("count")?.ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsPage1_CheckCount()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            var result = controller.GetEnergyIndicByAddr(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false) as JsonResult;
            List<EnergyTable>? energyTableRows = result?.Value as List<EnergyTable>;

            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), energyTableRows?.Count().ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsPage1_CheckLastRecordAddr()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            var result = controller.GetEnergyIndicByAddr(1,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false) as JsonResult;
            List<EnergyTable>? energyTableRows = result?.Value as List<EnergyTable>;

            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[0].ToString(),
                energyTableRows?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsPage2_CheckCount()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            var result = controller.GetEnergyIndicByAddr(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false) as JsonResult;
            List<EnergyTable>? energyTableRows = result?.Value as List<EnergyTable>;

            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER.ToString(), energyTableRows?.Count().ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsPage2_CheckLastRecordAddr()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            var result = controller.GetEnergyIndicByAddr(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false) as JsonResult;
            List<EnergyTable>? energyTableRows = result?.Value as List<EnergyTable>;

            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1].ToString(),
                energyTableRows?[KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER - 1].Address.ToString());
        }

        [Fact]
        public void GetEnergyIndicByAddr_GetRecordsPage2_CheckFirstRecordAddr()
        {
            var _logger = new Mock<ILogger<EnergyIndicationsController>>().Object;
            EnergyIndicationsController controller = new EnergyIndicationsController(Fixture.CreateContext(), _logger);

            var result = controller.GetEnergyIndicByAddr(2,
                KzmpEnergyTestingDbFixture.DEFAULT_ROWS_FOR_METER,
                KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES,
                false) as JsonResult;
            List<EnergyTable>? energyTableRows = result?.Value as List<EnergyTable>;

            Assert.Equal(KzmpEnergyTestingDbFixture.DEFAULT_ADDRESSES[1].ToString(),
                energyTableRows?[0].Address.ToString());
        }
    }
}
