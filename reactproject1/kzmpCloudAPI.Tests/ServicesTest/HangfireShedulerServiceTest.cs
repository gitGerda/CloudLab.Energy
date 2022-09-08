using Xunit;
using Moq;
using kzmpCloudAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using kzmpCloudAPI.Components.HangfireSheduler;
using kzmpCloudAPI.Components.IndicationsReading;
using System.Threading.Tasks;
using Hangfire;
using kzmpCloudAPI.Components.HangfireSheduler.UpdateShedulePart;
using kzmpCloudAPI.Tests.Helpers;
using HangfireJobsToRabbitLibrary.Jobs.IndicationsReading;
using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Tests.ServicesTest
{
    public class HangfireShedulerServiceTest
    {
        [Fact]
        public void CreateShedule_OnEmptyPeriodicityNullSheduleIdEmptySelectedMetersId_Exception()
        {
            //arrange
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _indications_reading_job_mock = new Mock<IIndicationsReadingJob>();

            var _update_shedule = new Mock<IUpdateShedule>();

            var _hangfire_sheduler = new Mock<HangfireShedulerService>(_indications_reading_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //assert
            //empty periodicity
            Assert.ThrowsAsync<Exception>(() => _hangfire_sheduler.Object.CreateShedule(new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = "",
                selected_meters_id = new List<int>() { 1, 2, 3, 4 },
                shedule_id = 1,
                shedule_name = ""
            }));
            //null shedule id
            Assert.ThrowsAsync<Exception>(() => _hangfire_sheduler.Object.CreateShedule(new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = "every day",
                selected_meters_id = new List<int>() { 1, 2, 3, 4 },
                shedule_id = null,
                shedule_name = ""
            }));
            //empty selected_meters_id
            Assert.ThrowsAsync<Exception>(() => _hangfire_sheduler.Object.CreateShedule(new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = "every day",
                selected_meters_id = new List<int>(),
                shedule_id = 0,
                shedule_name = ""
            }));
        }

        [Theory]
        [InlineData("every day")]
        [InlineData("every month")]
        [InlineData("every week")]
        public async void CreateShedule_OnValidPeriodicityValues_TrueResult(string periodicity)
        {
            //arrange
            const int _communic_point_id = 1;
            List<int> _selected_meters_id = new List<int>() { 1, 2, 3, 4 };
            SheduleCreateUpdateModel shedule = new SheduleCreateUpdateModel()
            {
                communic_point_id = _communic_point_id,
                periodicity = periodicity,
                selected_meters_id = _selected_meters_id,
                shedule_id = 0,
                shedule_name = "test",
                start_date = DateTime.Now.ToString()
            };
            JobCreateSettings _job_settings = new JobCreateSettings()
            {
                rabbit_server_address = "test",
                last_datetime_request = "test"
            };
            CommunicPoint _communic_point = new CommunicPoint();
            List<BrokerTaskMessage> _message = new List<BrokerTaskMessage>();

            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _indications_reading_job_mock = new Mock<IIndicationsReadingJob>();
            _indications_reading_job_mock.Setup(ex => ex.PushJobToRabbit(_job_settings, _message)).Returns(Task.FromResult(true));
            var _update_shedule = new Mock<IUpdateShedule>();

            var _hangfire_sheduler_mock = new Mock<HangfireShedulerService>(_indications_reading_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);
            _hangfire_sheduler_mock.Setup(ex => ex.GetCommunicPointInfo(_communic_point_id)).Returns(() => _communic_point);
            _hangfire_sheduler_mock.Setup(ex => ex.GetBrokerTaskMessageList(_selected_meters_id, shedule.shedule_id ?? -1, shedule.start_date)).Returns(() => _message);
            _hangfire_sheduler_mock.Setup(ex => ex.GetJobCreateSettings(shedule, _communic_point)).Returns(() => _job_settings);

            //act
            var result = await _hangfire_sheduler_mock.Object.CreateShedule(shedule);

            //assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("", "192.168.0.64")]
        [InlineData("192.168.0.64", "")]
        public async void CreateShedule_OnEmptyRabbitAndWebServerAddress_Exception(string rabbit_server_address, string web_server_address)
        {
            //arrange
            const int _communic_point_id = 1;
            List<int> _selected_meters_id = new List<int>() { 1, 2, 3, 4 };
            SheduleCreateUpdateModel shedule = new SheduleCreateUpdateModel()
            {
                communic_point_id = _communic_point_id,
                periodicity = "every day",
                selected_meters_id = _selected_meters_id,
                shedule_id = 0,
                shedule_name = "test",
                start_date = DateTime.Now.ToString()
            };
            JobCreateSettings _job_settings = new JobCreateSettings()
            {
                rabbit_server_address = rabbit_server_address,
                last_datetime_request = web_server_address
            };
            CommunicPoint _communic_point = new CommunicPoint();
            List<BrokerTaskMessage> _message = new List<BrokerTaskMessage>();

            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _indications_reading_job_mock = new Mock<IIndicationsReadingJob>();
            _indications_reading_job_mock.Setup(ex => ex.PushJobToRabbit(_job_settings, _message)).Returns(Task.FromResult(true));

            var _update_shedule = new Mock<IUpdateShedule>();

            var _hangfire_sheduler_mock = new Mock<HangfireShedulerService>(_indications_reading_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);
            _hangfire_sheduler_mock.Setup(ex => ex.GetCommunicPointInfo(_communic_point_id)).Returns(() => _communic_point);
            _hangfire_sheduler_mock.Setup(ex => ex.GetBrokerTaskMessageList(_selected_meters_id, shedule.shedule_id ?? -1, shedule.start_date)).Returns(() => _message);
            _hangfire_sheduler_mock.Setup(ex => ex.GetJobCreateSettings(shedule, _communic_point)).Returns(() => _job_settings);

            //assert
            await Assert.ThrowsAsync<Exception>(() => _hangfire_sheduler_mock.Object.CreateShedule(shedule));
        }
        [Fact]
        public async void UpdateShedule_OnNullSheduleId_Exception()
        {
            //----
            var _shedule = new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = "every day",
                selected_meters_id = new List<int>() { 1, 2, 3, 4 },
                shedule_name = "test"
            };
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();

            var _hangfire_sheduler_mock = new Mock<HangfireShedulerService>(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            await Assert.ThrowsAsync<Exception>(() => _hangfire_sheduler_mock.Object.UpdateShedule(_shedule));
        }
        [Theory]
        [InlineData("every day")]
        [InlineData("every week")]
        [InlineData("every month")]
        public async void UpdateShedule_OnValidPeriodicity_True(string periodicity)
        {
            //----
            var _shedule = new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = periodicity,
                selected_meters_id = new List<int>() { 1, 2, 3, 4 },
                shedule_name = "test",
                shedule_id = 1
            };
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();

            var _hangfire_sheduler_mock = new Mock<HangfireShedulerService>(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            var result = await _hangfire_sheduler_mock.Object.UpdateShedule(_shedule);

            //----
            Assert.False(result);
        }
        [Fact]
        public async void UpdateShedule_OnEmptySelectedMetersId_Exception()
        {
            //----
            var _shedule = new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = "every day",
                selected_meters_id = new List<int>(),
                shedule_name = "test",
                shedule_id = 1
            };
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();

            var _hangfire_sheduler_mock = new Mock<HangfireShedulerService>(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            await Assert.ThrowsAsync<Exception>(() => _hangfire_sheduler_mock.Object.UpdateShedule(_shedule));
        }
        [Fact]
        public async void UpdateShedule_OnInvoke_VerifySheduleMetersUpdateFuncInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            _update_shedule.Setup(ex => ex.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id));

            var _hangfire_sheduler = new HangfireShedulerService(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            var result = await _hangfire_sheduler.UpdateShedule(_shedule);

            //----
            _update_shedule.Verify(ex => ex.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id));
        }
        [Fact]
        public async void UpdateShedule_OnInvoke_VerifyProcessCommunicPointChangeFuncInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            _update_shedule.Setup(ex => ex.ProcessCommunicPointChange(_shedule.shedule_id ?? 1, _shedule.communic_point_id));

            var _hangfire_sheduler = new HangfireShedulerService(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            var result = await _hangfire_sheduler.UpdateShedule(_shedule);

            //----
            _update_shedule.Verify(ex => ex.ProcessCommunicPointChange(_shedule.shedule_id ?? 1, _shedule.communic_point_id));
        }
        [Fact]
        public async void UpdateShedule_OnInvoke_VerifyUpdateSheduleCreatingDateInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            var _hangfire_sheduler = new HangfireShedulerService(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            var result = await _hangfire_sheduler.UpdateShedule(_shedule);

            //----
            _update_shedule.Verify(ex => ex.UpdateSheduleCreatingDate(_shedule.shedule_id ?? 1));
        }
        [Fact]
        public async void UpdateShedule_OnInvoke_VerifyUpdatePeriodicityInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            var _hangfire_sheduler = new HangfireShedulerService(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            var result = await _hangfire_sheduler.UpdateShedule(_shedule);

            //----
            _update_shedule.Verify(ex => ex.UpdatePeriodicity(_shedule.shedule_id ?? 1, _shedule.periodicity));
        }
        /* [Fact]
         public async void DeleSheduleFromSheduler_OnNullSheduleId_FalseResult()
         {
             //----
             var _shedule = SheduleHelpers._get_default_shedule();
             _shedule.shedule_id = null;
             var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
             var _database_mock = new Mock<kzmp_energyContext>();
             var _conf_mock = new Mock<IConfiguration>();
             var _update_shedule = new Mock<IUpdateShedule>();
             var _hangfire_sheduler = new HangfireShedulerService(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

             //----
             var result = _hangfire_sheduler.DeleteSheduleFromSheduler(_shedule.shedule_id ?? -1);

             //----
             Assert.False(result);
         }*/

        [Theory]
        [InlineData(1)]
        public void DeleteSheduleFull_OnInvoke_VerifyCheckSheduleExistenceFuncInvoke(int shedule_id)
        {
            //----
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            var _hangfire_sheduler = new Mock<HangfireShedulerService>(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);

            //----
            var result = _hangfire_sheduler.Object.DeleteSheduleFull(shedule_id);

            //----
            _hangfire_sheduler.Verify(t => t.CheckSheduleExistence(shedule_id), Times.Once);
        }
        [Theory]
        [InlineData(1)]
        public void DeleteSheduleFull_OnCheckSheduleExistencsTrueResult_VerifyDeleteSheduleFromShedulerInvoke(int shedule_id)
        {
            //----
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            var _hangfire_sheduler = new Mock<HangfireShedulerService>(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);
            _hangfire_sheduler.Setup(t => t.CheckSheduleExistence(shedule_id)).Returns(true);

            //----
            var result = _hangfire_sheduler.Object.DeleteSheduleFull(shedule_id);

            //----
            _hangfire_sheduler.Verify(t => t.DeleteSheduleFromSheduler(shedule_id));

        }
        [Theory]
        [InlineData(1)]
        public void DeleteSheduleFull_OnCheckSheduleExistencsTrueResult_VerifyDeleteSheduleFromDb(int shedule_id)
        {
            //----
            var _indic_read_job_mock = new Mock<IIndicationsReadingJob>();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _conf_mock = new Mock<IConfiguration>();
            var _update_shedule = new Mock<IUpdateShedule>();
            var _hangfire_sheduler = new Mock<HangfireShedulerService>(_indic_read_job_mock.Object, _database_mock.Object, _conf_mock.Object, _update_shedule.Object);
            _hangfire_sheduler.Setup(t => t.CheckSheduleExistence(shedule_id)).Returns(true);

            //----
            var result = _hangfire_sheduler.Object.DeleteSheduleFull(shedule_id);

            //----
            _hangfire_sheduler.Verify(t => t.DeleteSheduleFromDb(shedule_id));

        }
    }
}
