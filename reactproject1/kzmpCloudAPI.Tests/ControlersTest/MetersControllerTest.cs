using kzmpCloudAPI.Controllers.Meters;
using kzmpCloudAPI.Tests.Fixtures;
using kzmpCloudAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.ControlersTest
{
    public class MetersControllerTest : IClassFixture<KzmpEnergyTestingDbFixture>
    {
        public KzmpEnergyTestingDbFixture _fixture_of_database;
        public MetersControllerTest(KzmpEnergyTestingDbFixture fixture_of_database)
        {
            _fixture_of_database = fixture_of_database;
        }
        //-----------------------------------------------------------------------------
        //CreateUpdateCompany
        [Fact]
        public void CreateUpdateCompany_OnExistingCompanyNameChange_NameChangedInMsgNumbersTable()
        {
            //---------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _existing_company = _database.MsgNumbers.FirstOrDefault();
            _existing_company.CompanyName = "Changed Company Name Here";
            var _target = new MetersController(_fixture_of_database.CreateContext(), _logger);
            //--------
            var _result = _target.CreateUpdateCompany(_existing_company);
            //-------
            var _database2 = _fixture_of_database.CreateContext();
            var _changed = _database2.MsgNumbers.Where(t => t.CompanyInn == _existing_company.CompanyInn).Select(t => t.CompanyName).FirstOrDefault();
            Assert.Equal(_existing_company.CompanyName, _changed);
        }
        [Fact]
        public void CreateUpdateCompany_OnCompanyWhichExistInMetersTableNameChange_NameChanged()
        {
            //---------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _inn = _database.Meters.Select(t => t.Inn).FirstOrDefault();
            var _existing_company = _database.MsgNumbers.Where(t => t.CompanyInn == _inn).Select(t => t).FirstOrDefault();
            _existing_company.CompanyName = "Changed Company Name Here";
            var _target = new MetersController(_fixture_of_database.CreateContext(), _logger);
            //--------
            var _result = _target.CreateUpdateCompany(_existing_company);
            //-------
            var _database2 = _fixture_of_database.CreateContext();
            var _changed = _database2.MsgNumbers.Where(t => t.CompanyInn == _existing_company.CompanyInn).Select(t => t.CompanyName).FirstOrDefault();
            var _changed2 = _database2.Meters.Where(t => t.Inn == _existing_company.CompanyInn).Select(t => t.CompanyName).FirstOrDefault();
            Assert.Equal(_existing_company.CompanyName, _changed2);
            Assert.Equal(_existing_company.CompanyName, _changed);
        }
        [Fact]
        public void CreateUpdateCompany_OnNewCompany_AddingNewCompanyToDB()
        {
            //---------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _existing_company = _database.MsgNumbers.FirstOrDefault();
            _existing_company.CompanyName = "Changed Company Name Here";
            _existing_company.CompanyInn = "new company inn";
            var _target = new MetersController(_fixture_of_database.CreateContext(), _logger);
            //--------
            var _result = _target.CreateUpdateCompany(_existing_company);
            //-------
            var _database2 = _fixture_of_database.CreateContext();
            var _added = _database2.MsgNumbers.Where(t => t.CompanyInn == _existing_company.CompanyInn).Select(t => t.CompanyName).FirstOrDefault();
            Assert.Equal(_existing_company.CompanyName, _added);
        }
        //-------------------------------------------------------------------
        //DeleteCompany
        [Fact]
        public void DeleteCompany_OnNoExistCompany_BadRequestResult()
        {
            //--------------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _target = new MetersController(_database, _logger);
            //--------------
            var _result = _target.DeleteCompany("no exist company inn");
            //--------------
            Assert.IsType<BadRequestResult>(_result);
        }
        [Fact]
        public void DeleteCompany_OnInnWhichExistOnMetersTable_BadRequestResult()
        {
            //--------------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _inn = _database.Meters.Select(t => t.Inn).First();
            var _target = new MetersController(_database, _logger);
            //--------------
            var _result = _target.DeleteCompany(_inn);
            //-------------
            Assert.IsType<BadRequestResult>(_result);
        }
        [Fact]
        public void DeleteCompany_OnInnWhichNonExistOnMetersTable_DeleteCompany()
        {
            //--------------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _inn_from_meters_table = _database.Meters.Select(t => t.Inn).Distinct().ToList();
            var _inn_from_msg_number_table = _database.MsgNumbers.Select(t => t.CompanyInn).ToList();
            var _unic_inn = _inn_from_msg_number_table.Except(_inn_from_meters_table).ToList().First();
            var _target = new MetersController(_database, _logger);
            //---------------------------
            var _result = _target.DeleteCompany(_unic_inn);
            //----------------------------
            Assert.IsType<OkResult>(_result);
            Assert.False(_database.MsgNumbers.Where(t => t.CompanyInn == _unic_inn).Any());
        }
        //------------------------------------------------------------------------------
        //CreateMeter
        [Fact]
        public void CreateMeter_OnExistMeter_BadRequestResult()
        {
            //------------------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _exist_meter = _database.Meters.First();
            var _target = new MetersController(_database, _logger);
            //-----------------
            var _result = _target.CreateMeter(_exist_meter);
            //-----------------
            Assert.IsType<BadRequestResult>(_result);
        }
        [Fact]
        public void CreateMeter_OnNonExistMeter_OkResult()
        {
            //------------------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _meter = kzmpCloudApiGeneralTestHelper.CreateDefaultMeter();
            _meter.Sim = "CreateMeter_OnNonExistMeter_NewMeterinDb";
            var _target = new MetersController(_database, _logger);
            //-----------------
            var _result = _target.CreateMeter(_meter);
            //-----------------
            Assert.IsType<OkResult>(_result);
            var _added = _database.Meters.Where(t => t.Sim == _meter.Sim).Select(t => t).First();
            _database.Meters.Remove(_added);
            _database.SaveChanges();
        }

        //---------------------------------------------------------------------------
        //DeleteMeter
        [Theory]
        [InlineData(1)]
        public void DeleteMeter_OnMeterWhichExistOnSomeShedule_BadRequestResult(int meter_id)
        {
            //------------------
            var _logger = new Mock<ILogger<MetersController>>().Object;
            var _database = _fixture_of_database.CreateContext();
            var _target = new MetersController(_database, _logger);
            //------------------
            var _result = _target.DeleteMeter(meter_id);
            //------------------
            Assert.IsType<BadRequestResult>(_result);
        }
    }
}
