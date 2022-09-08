using kzmpCloudAPI.Controllers.CommunicPoints;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Services.RabbitMQService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.ControlersTest
{
    public class CommunicPointsControllerTest
    {
        [Theory]
        [InlineData(1, "COM1", "")]
        public void ChangeCommunicPoint_OnInvoke_VerifyCheckCommPointExistenseByIdInvoke(int communic_point_id, string port, string desc)
        {
            //----
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);

            //----
            var result = _controller_mock.Object.ChangeCommunicPoint(communic_point_id: communic_point_id,
                port: port,
                desc: desc);

            //----
            _controller_mock.Verify(t => t.CheckCommPointExistencseById(communic_point_id), Times.Once());
        }
        [Theory]
        [InlineData(1, "COM1", "")]
        public void ChangeCommunicPoint_OnInvoke_VerifyGetCommunicPointInvoke(int communic_point_id, string port, string desc)
        {
            //----
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);
            _controller_mock.Setup(t => t.CheckCommPointExistencseById(communic_point_id)).Returns(true);

            //----
            var result = _controller_mock.Object.ChangeCommunicPoint(communic_point_id: communic_point_id,
                port: port,
                desc: desc);

            //----
            _controller_mock.Verify(t => t.GetCommunicPoint(communic_point_id), Times.Once());
        }
        [Theory]
        [InlineData("COM1", "COM3")]
        public void ChangeCommunicPoint_OnDifferentPorts_VerifyCreatePortConfigureRabbitMsgInvoke(string port1, string port2)
        {
            //----
            var _communic_point = _GetDefaultCommunicPoint();
            _communic_point.Port = port1;
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);
            _controller_mock.Setup(t => t.CheckCommPointExistencseById(_communic_point.Id)).Returns(true);
            _controller_mock.Setup(t => t.GetCommunicPoint(_communic_point.Id)).Returns(_communic_point);

            //----
            var result = _controller_mock.Object.ChangeCommunicPoint(_communic_point.Id, port2, _communic_point.Description);

            //----
            _controller_mock.Verify(t => t.CreatePortConfigureRabbitMsg(_communic_point.Name, port2));
        }
        [Theory]
        [InlineData(1)]
        public void DeleteCommunicPoint_OnInvoke_VerifyCheckCommPointExistencseByIdInvoke(int communic_point_id)
        {
            //---- 
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);

            //----
            var result = _controller_mock.Object.DeleteCommunicPoint(communic_point_id);

            //----
            _controller_mock.Verify(t => t.CheckCommPointExistencseById(communic_point_id), Times.Once);
        }
        [Theory]
        [InlineData(1)]
        public void DeleteCommunicPoint_OnInvoke_VerifyGetShedulesByCommunicPointIdInvoke(int communic_point_id)
        {
            //---- 
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);
            _controller_mock.Setup(t => t.CheckCommPointExistencseById(communic_point_id)).Returns(true);

            //----
            var result = _controller_mock.Object.DeleteCommunicPoint(communic_point_id);

            //----
            _controller_mock.Verify(t => t.GetShedulesByCommunicPointId(communic_point_id));
        }
        [Theory]
        [InlineData(1)]
        public void DeleteCommunicPoint_OnGetShedulesByCommunicPointIdNotNullResult_ResultTypeJsonResult(int communic_point_id)
        {
            //---- 
            var _shedules = _GetListOfShedules();
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);
            _controller_mock.Setup(t => t.CheckCommPointExistencseById(communic_point_id)).Returns(true);
            _controller_mock.Setup(t => t.GetShedulesByCommunicPointId(communic_point_id)).Returns(_shedules);

            //----
            var result = _controller_mock.Object.DeleteCommunicPoint(communic_point_id);

            //----
            Assert.IsType<JsonResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public void DeleteCommunicPoint_OnGetShedulesByCommunicPointIdNullResult_VerifyDeleteCommunicPointFromDatabaseInvoke(int communic_point_id)
        {
            //---- 
            var _shedules = _GetListOfShedules();
            var _logger = new Mock<ILogger<CommunicPointsController>>().Object;
            var _database = new Mock<kzmp_energyContext>().Object;
            var _rabbit = new Mock<IRabbitMQPersistentConnection>().Object;
            var _conf = new Mock<IConfiguration>().Object;
            var _publisher = new Mock<IRabbitPublisher>().Object;
            var _controller_mock = new Mock<CommunicPointsController>(_logger, _database, _publisher);
            _controller_mock.Setup(t => t.CheckCommPointExistencseById(communic_point_id)).Returns(true);
            _controller_mock.Setup(t => t.GetShedulesByCommunicPointId(communic_point_id)).Returns(() => null);

            //----
            var result = _controller_mock.Object.DeleteCommunicPoint(communic_point_id);

            //----
            _controller_mock.Verify(t => t.DeleteCommunicPointFromDatabase(communic_point_id), Times.Once());
        }
        public CommunicPoint _GetDefaultCommunicPoint()
        {
            return new CommunicPoint()
            {
                Id = 1,
                Name = "TestCommunicPoint",
                Port = "COM1",
                Description = "Desc"
            };
        }
        public List<Shedule> _GetListOfShedules()
        {
            return new List<Shedule>()
            {
                new Shedule()
                {
                    CommunicPointId = 1,
                    CreatingDate=DateTime.Now,
                    Id=1,
                    Name="Test1",
                    Shedule1="every day",
                    Status=true
                },
                new Shedule()
                {
                    CommunicPointId = 1,
                    CreatingDate=DateTime.Now,
                    Id=1,
                    Name="Test2",
                    Shedule1="every day",
                    Status=true
                }
            };
        }

    }
}
