using kzmpCloudAPI.Components.HangfireSheduler.UpdateShedulePart;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace kzmpCloudAPI.Tests.ComponentsTest.UpdateShedulePart
{
    public class UpdateSheduleTest
    {
        [Fact]
        public void SheduleMetersTableUpdate_OnInvoke_VerifyCompareToListInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _database_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_mock = new Mock<UpdateShedule>(_database_mock.Object);
            _update_shedule_mock.Setup(ex => ex.GetCurrentSelectedMeters(_shedule.shedule_id ?? 1)).Returns(_shedule.selected_meters_id);

            //----
            _update_shedule_mock.Object.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id);

            //----
            _update_shedule_mock.Verify(ex => ex.CompareTwoList(_shedule.selected_meters_id, _shedule.selected_meters_id));
        }
        [Fact]
        public void ShedulesMetersTableUpdate_OnInvoke_VerifyGetCurrentSelectedMetersInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _db_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_mock = new Mock<UpdateShedule>(_db_mock.Object);

            //----
            _update_shedule_mock.Object.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id);

            //----
            _update_shedule_mock.Verify(ex => ex.GetCurrentSelectedMeters(_shedule.shedule_id ?? 1));
        }
        [Fact]
        public void ShedulesMetersTableUpdate_OnGetCurrrentSelectedMetersNullResult_VerifyWriteToDbSelectedMetersInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _db_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_mock = new Mock<UpdateShedule>(_db_mock.Object);
            _update_shedule_mock.Setup(ex => ex.GetCurrentSelectedMeters(_shedule.shedule_id ?? 1)).Returns(() => null);

            //----
            _update_shedule_mock.Object.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id);

            //----
            _update_shedule_mock.Verify(ex => ex.WriteToDbSelectedMeters(_shedule.shedule_id ?? 1, _shedule.selected_meters_id));
        }
        [Fact]
        public void SheduleMetersTableUpdate_OnCompareTwoListFalseResult_VerifyRemoveSheduleMetersRecordInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _list2 = new List<int>() { -1 };
            _list2.AddRange(_shedule.selected_meters_id.ToArray());
            var _db_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_mock = new Mock<UpdateShedule>(_db_mock.Object);
            _update_shedule_mock.Setup(ex => ex.GetCurrentSelectedMeters(_shedule.shedule_id ?? 1)).Returns(_list2);

            //----
            _update_shedule_mock.Object.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id);

            //----
            _update_shedule_mock.Verify(ex => ex.RemoveSheduleMetersRecords(_shedule.shedule_id ?? 1));

        }
        [Fact]
        public void SheduleMetersTableUpdate_OnCompareTwoListFalseResult_VerifyWriteToDbSelectedMetersInvoke()
        {
            //----
            var _shedule = SheduleHelpers._get_default_shedule();
            var _list2 = new List<int>() { -1 };
            _list2.AddRange(_shedule.selected_meters_id.ToArray());
            var _db_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_mock = new Mock<UpdateShedule>(_db_mock.Object);
            _update_shedule_mock.Setup(ex => ex.GetCurrentSelectedMeters(_shedule.shedule_id ?? 1)).Returns(_list2);

            //----
            _update_shedule_mock.Object.SheduleMetersTableUpdate(_shedule.shedule_id ?? 1, _shedule.selected_meters_id);

            //----
            _update_shedule_mock.Verify(ex => ex.WriteToDbSelectedMeters(_shedule.shedule_id ?? 1, _shedule.selected_meters_id));

        }
        [Fact]
        public void CompareTwoList_OnSameLists_True()
        {
            //----
            var list1 = new List<int>() { 1, 2, 3, 4, 5 };
            var list2 = new List<int>() { 5, 4, 3, 2, 1 };
            var _database_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_obj = new UpdateShedule(_database_mock.Object);

            //----
            var result = _update_shedule_obj.CompareTwoList(list1, list2);

            //----
            Assert.True(result);
        }
        [Fact]
        public void CompareTwoList_OnDifferentLists_False()
        {
            //----
            var list1 = new List<int>() { 1, 2, 3, 4, 5 };
            var list2 = new List<int>() { 6, 7, 8, 9, 0 };
            var _database_mock = new Mock<kzmp_energyContext>();
            var _update_shedule_obj = new UpdateShedule(_database_mock.Object);

            //----
            var result = _update_shedule_obj.CompareTwoList(list1, list2);

            //----
            Assert.False(result);
        }
    }
}
