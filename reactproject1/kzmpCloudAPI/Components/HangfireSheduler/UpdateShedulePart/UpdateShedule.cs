using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Components.HangfireSheduler.UpdateShedulePart
{
    public class UpdateShedule : IUpdateShedule
    {
        kzmp_energyContext _database;
        public UpdateShedule(kzmp_energyContext database)
        {
            _database = database;
        }
        public void UpdateSheduleCreatingDate(int shedule_id)
        {
            var _shedule = _database.Shedules.FirstOrDefault(t => t.Id == shedule_id);
            if (_shedule != null)
            {
                _shedule.CreatingDate = DateTime.Now;
                _database.SaveChanges();
            }
        }
        public void ProcessCommunicPointChange(int shedule_id, int communic_point_id)
        {
            var _shedule_record = (from t in _database.Shedules
                                   where t.Id == shedule_id
                                   select t).FirstOrDefault();
            if (_shedule_record != null)
            {
                _shedule_record.CommunicPointId = communic_point_id;
                _database.SaveChanges();
            }
        }
        public void SheduleMetersTableUpdate(int shedule_id, List<int> selected_meters_id)
        {
            var _cur_selected_meters = GetCurrentSelectedMeters(shedule_id);
            if (_cur_selected_meters == null)
            {
                //write selected_meters_id to database
                WriteToDbSelectedMeters(shedule_id, selected_meters_id);
            }
            else
            {
                //compare _cur_selected_meters with selected_meters_id(param)
                if (!CompareTwoList(_cur_selected_meters, selected_meters_id))
                {
                    //clear records in SheduleMeters table with shedule id = sheduele_id
                    RemoveSheduleMetersRecords(shedule_id);
                    //write selected_meters_id to SheduleMeters Table
                    WriteToDbSelectedMeters(shedule_id, selected_meters_id);
                }
            }
        }
        public void UpdatePeriodicity(int shedule_id, string periodicity)
        {
            var _shedule = _database.Shedules.FirstOrDefault(t => t.Id == shedule_id);
            if (_shedule != null)
            {
                _shedule.Shedule1 = periodicity;
                _database.SaveChanges();
            }
        }

        public virtual void RemoveSheduleMetersRecords(int shedule_id)
        {
            if (!(from t in _database.ShedulesMeters
                  where t.SheduleId == shedule_id
                  select t).Any())
                return;

            var _shedule_meters_table_records = (from t in _database.ShedulesMeters
                                                 where t.SheduleId == shedule_id
                                                 select t).ToList();

            _database.ShedulesMeters.RemoveRange(_shedule_meters_table_records);
            _database.SaveChanges();
        }
        public virtual void WriteToDbSelectedMeters(int shedule_id, List<int> selected_meters_id)
        {
            foreach (var meter_id in selected_meters_id)
            {
                var _shedule_meters_record = new ShedulesMeter()
                {
                    MeterId = meter_id,
                    SheduleId = shedule_id
                };
                _database.ShedulesMeters.Add(_shedule_meters_record);
            }
            _database.SaveChanges();
        }
        public virtual List<int>? GetCurrentSelectedMeters(int shedule_id)
        {
            if (!(from t in _database.ShedulesMeters
                  where t.SheduleId == shedule_id
                  select t.MeterId).Any())
                return null;

            var _cur_selected_meters = (from t in _database.ShedulesMeters
                                        where t.SheduleId == shedule_id
                                        select t.MeterId).ToList();

            return _cur_selected_meters;
        }
        public virtual bool CompareTwoList<T>(List<T> list1, List<T> list2)
        {
            var ex_1 = list1.Except(list2).Count();
            var ex_2 = list2.Except(list1).Count();

            if (ex_1 > 0 || ex_2 > 0)
                return false;

            return true;
        }
    }
}
