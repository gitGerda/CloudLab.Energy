using kzmpCloudAPI.Components.IndicationsReading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using kzmpCloudAPI.Controllers.Shedules;
using Microsoft.Extensions.Configuration;
using kzmpCloudAPI.Components.HangfireSheduler;
using Microsoft.AspNetCore.Mvc;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Database.EF_Core;

namespace kzmpCloudAPI.Tests.ControlersTest.Shedules
{
    public class ShedulesControllerTest
    {
        [Theory]
        [InlineData("test")]
        public async void CreateUpdateShedulePost_OnInValidPeriodicity_BadRequestResult(string periodicity)
        {
            //----
            var _data = _getDefaultValidData();
            _data.periodicity = periodicity;
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);

            //----
            var result = await _shedules_controller_mock.Object.CreateUpdateShedule(_data);

            //----
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async void CreateUpdateShedule_OnNullSheduleId_VerifyWriteNewSheduleToDbInvoke()
        {
            //----
            var _data = _getDefaultValidData();
            _data.shedule_id = null;
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);
            _shedules_controller_mock.Setup(ex => ex.CheckSheduleExisting(_data.shedule_name)).Returns(false);
            //----
            var result = await _shedules_controller_mock.Object.CreateUpdateShedule(_data);

            //----
            _shedules_controller_mock.Verify(ex => ex.WriteNewSheduleToDb(_data));
        }
        [Fact]
        public async void CreateUpdateShedule_OnNullSheduleId_VerifyCreateSheduleInvoke()
        {
            //----
            var _data = _getDefaultValidData();
            _data.shedule_id = null;
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);
            _shedules_controller_mock.Setup(ex => ex.CheckSheduleExisting(_data.shedule_name)).Returns(false);
            //----
            var result = await _shedules_controller_mock.Object.CreateUpdateShedule(_data);

            //----
            _hangfire_sheduler_mock.Verify(ex => ex.CreateShedule(_data));
        }
        [Fact]
        public async void CreateUpdateShedule_OnNoNullSheduleId_VerifyUpdateSheduleInvoke()
        {
            //----
            var _data = _getDefaultValidData();
            _data.shedule_id = 1;
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);
            _shedules_controller_mock.Setup(ex => ex.CheckSheduleExisting(_data.shedule_name)).Returns(true);
            //----
            var result = await _shedules_controller_mock.Object.CreateUpdateShedule(_data);

            //----
            _hangfire_sheduler_mock.Verify(ex => ex.UpdateShedule(_data));
        }
        [Fact]
        public async void ChangeSheduleStatus_OnInvoke_VerifyChangeStatusFieldInvoke()
        {
            //----
            var _data = _getDefaultValidData();
            _data.shedule_id = 1;
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);

            //---- 
            var result = _shedules_controller_mock.Object.ChangeSheduleStatus(_data.shedule_id ?? 1, true);

            //----
            _shedules_controller_mock.Verify(ex => ex.ChangeStatusField(_data.shedule_id ?? 1, true));
        }
        [Fact]
        public void ChangeSheduleStatus_OnInvoke_VerifyGetSheduleCreateUpdateModelInvoke()
        {
            //----
            var _data = _getDefaultShedule();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);
            _shedules_controller_mock.Setup(t => t.ChangeStatusField(_data.Id, true)).Returns(() => _data);

            //---- 
            var result = _shedules_controller_mock.Object.ChangeSheduleStatus(_data.Id, true);

            //----
            _shedules_controller_mock.Verify(t => t.GetSheduleCreateUpdateModel(_data));
        }
        [Fact]
        public void ChangeSheduleStatus_OnTrueStatus_VerifyCreateSheduleInvoke()
        {
            //----
            var _data = _getDefaultShedule();
            var _shedule_create_model = _getDefaultValidData();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);
            _shedules_controller_mock.Setup(t => t.ChangeStatusField(_data.Id, true)).Returns(() => _data);
            _shedules_controller_mock.Setup(t => t.GetSheduleCreateUpdateModel(_data)).Returns(_shedule_create_model);

            //---- 
            var result = _shedules_controller_mock.Object.ChangeSheduleStatus(_data.Id, true);

            //----
            _hangfire_sheduler_mock.Verify(t => t.CreateShedule(_shedule_create_model));
        }
        [Fact]
        public void ChangeSheduleStatus_OnFalseStatus_VerifyDeleteSheduleFromSheduler()
        {
            //----
            var _data = _getDefaultShedule();
            var _data2 = _getDefaultShedule();
            _data2.Status = false;
            var _shedule_create_model = _getDefaultValidData();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);
            _shedules_controller_mock.Setup(t => t.ChangeStatusField(_data.Id, false)).Returns(() => _data2);
            _shedules_controller_mock.Setup(t => t.GetSheduleCreateUpdateModel(_data2)).Returns(_shedule_create_model);

            //---- 
            var result = _shedules_controller_mock.Object.ChangeSheduleStatus(_data.Id, false);

            //----
            _hangfire_sheduler_mock.Verify(t => t.DeleteSheduleFromSheduler(_shedule_create_model.shedule_id ?? -1));

        }
        [Theory]
        [InlineData(1)]
        public void DeleteShedule_OnInvoke_VerifyDeleteSheduleFullInvoke(int shedule_id)
        {
            //----
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _hangfire_sheduler_mock = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>();
            var _shedules_controller_mock = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _hangfire_sheduler_mock.Object);

            //----
            _shedules_controller_mock.Object.DeleteShedule(shedule_id);

            //----
            _hangfire_sheduler_mock.Verify(t => t.DeleteSheduleFull(shedule_id));

        }

        [Theory]
        [InlineData(10, 20)]
        public void GetAllLogs_OnNullResultFromDatabase_BadResultReturn(int skip, int take)
        {
            //----
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _sheduler = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>().Object;
            var _controller = new ShedulesController(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _sheduler);
            //----
            var result = _controller.GetAllLogs(skip, take);
            //----
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData(10, 20)]
        public void GetAllLogs_OnNotNullResultFromDatabase_VerifyGetResponseToLogsRequestInvoke(int skip, int take)
        {
            //----
            var _sheduleLogsList = _getDefaultListOfSheduleLogs();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _sheduler = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>().Object;
            var _controller = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _sheduler);
            _controller.Setup(t => t._GetDataFromShedulesLogsTable(skip, take)).Returns(_sheduleLogsList);
            //----
            var result = _controller.Object.GetAllLogs(skip, take);
            //----
            _controller.Verify(t => t._getResponseToLogsRequest(_sheduleLogsList));
        }

        [Theory]
        [InlineData(1, 10, 20)]
        public void GetLogsBySheduleID_OnNullResultFromDatabase_BadResultReturn(int shedule_id, int skip, int take)
        {
            //----
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _sheduler = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>().Object;
            var _controller = new ShedulesController(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _sheduler);
            //----
            var result = _controller.GetLogsBySheduleId(shedule_id, skip, take);
            //----
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData(1, 10, 20)]
        public void GetLogsBySheduleID_OnNotNullResultFromDatabase_VerifyGetResponseToLogsRequestInvoke(int shedule_id, int skip, int take)
        {
            //----
            var _shedulesLogsList = _getDefaultListOfSheduleLogs();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _sheduler = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>().Object;
            var _controller = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _sheduler);
            _controller.Setup(t => t._getDataFromSheduleLogsTableBySheduleId(shedule_id, skip, take)).Returns(_shedulesLogsList);
            //----
            var result = _controller.Object.GetLogsBySheduleId(shedule_id, skip, take);
            //----
            _controller.Verify(t => t._getResponseToLogsRequest(_shedulesLogsList));
        }
        [Fact]
        public void ResetAlLLogs_OnInvoke_VerifyResetLogsInvoke()
        {
            //----
            var _shedulesLogsList = _getDefaultListOfSheduleLogs();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _logger_mock = new Mock<ILogger<ShedulesController>>();
            var _conf_mock = new Mock<IConfiguration>();
            var _sheduler = new Mock<IHangfireSheduler<SheduleCreateUpdateModel>>().Object;
            var _controller = new Mock<ShedulesController>(_database_mock.Object, _logger_mock.Object, _conf_mock.Object, _sheduler);

            //----
            var result = _controller.Object.ResetAllLogs();

            //----
            _controller.Verify(t => t._resetLogs());
        }

        private List<ShedulesLog> _getDefaultListOfSheduleLogs()
        {
            return new List<ShedulesLog>()
            {
                _getDefaultShedulesLogObj(),
                _getDefaultShedulesLogObj(),
                _getDefaultShedulesLogObj(),
                _getDefaultShedulesLogObj()
            };
        }
        private ShedulesLog _getDefaultShedulesLogObj()
        {
            return new ShedulesLog()
            {
                Id = 1,
                DateTime = DateTime.Now,
                Description = "test",
                SheduleId = 2,
                Status = "ok"
            };
        }
        private Shedule _getDefaultShedule()
        {
            return new Shedule()
            {
                CommunicPointId = 1,
                CreatingDate = DateTime.Now,
                Id = 1,
                Name = "test",
                Shedule1 = "every day",
                Status = true
            };
        }
        private SheduleCreateUpdateModel _getDefaultValidData()
        {
            return new SheduleCreateUpdateModel()
            {
                communic_point_id = 1,
                periodicity = "every day",
                selected_meters_id = new List<int> { 1, 2, 3, 4, 5 },
                shedule_id = 1,
                shedule_name = "test_shedule_name",
                status = true
            };
        }
    }
}
