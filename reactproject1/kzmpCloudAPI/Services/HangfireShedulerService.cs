using kzmpCloudAPI.Components.IndicationsReading;
using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using kzmpCloudAPI.Components.HangfireSheduler;
using kzmpCloudAPI.Components.HangfireSheduler.UpdateShedulePart;
using HangfireJobsToRabbitLibrary.Jobs.IndicationsReading;
using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;

namespace kzmpCloudAPI.Services
{
    public class HangfireShedulerService : IHangfireSheduler<SheduleCreateUpdateModel>
    {
        private kzmp_energyContext _database;
        private IConfiguration _conf;
        private IIndicationsReadingJob _job;
        private IUpdateShedule _update_shedule;
        public HangfireShedulerService(IIndicationsReadingJob job, kzmp_energyContext database, IConfiguration conf, IUpdateShedule update_shedule)
        {
            _database = database;
            _conf = conf;
            _job = job;
            _update_shedule = update_shedule;
        }
        public async Task<bool> CreateShedule(SheduleCreateUpdateModel shedule)

        {
            if (shedule.periodicity == "" || shedule.shedule_id == null || shedule.selected_meters_id.Count == 0)
                throw new Exception("Invalid parameters");

            if (shedule.periodicity != "every day" && shedule.periodicity != "every week" && shedule.periodicity != "every month")
                throw new Exception("Invalid periodicity");

            var communic_point = GetCommunicPointInfo(communic_point_id: shedule.communic_point_id);
            var _message = GetBrokerTaskMessageList(selected_meters_id: shedule.selected_meters_id, shedule_id: shedule.shedule_id ?? -1, shedule_start_date: shedule?.start_date ?? DateTime.Now.ToString());
            var _job_settings = GetJobCreateSettings(shedule: shedule,
                communic_point: communic_point);

            if (_job_settings.rabbit_server_address == "" || _job_settings.last_datetime_request == "")
                throw new Exception("Invalid parameters [RABBIT_SERVER_ADDRESS] or [LAST_DATETIME_REQUEST]");

            var result = await _job.PushJobToRabbit(_job_settings, _message);

            return result;
        }

        public virtual bool DeleteSheduleFromSheduler(int shedule_id)
        {
            RecurringJob.RemoveIfExists(shedule_id.ToString());
            return true;
        }

        public async Task<bool> UpdateShedule(SheduleCreateUpdateModel shedule)
        {
            //validate parameters
            if (shedule.shedule_id == null)
                throw new Exception("Error: invalid shedule_id");

            if (shedule.periodicity != "every day" && shedule.periodicity != "every week" && shedule.periodicity != "every month")
                throw new Exception("Invalid periodicity");

            if (shedule.selected_meters_id.Count == 0)
                throw new Exception("Error: invalid shedule_meters_id");

            //check and update SheduleMeters table
            _update_shedule.SheduleMetersTableUpdate(shedule.shedule_id ?? -1, shedule.selected_meters_id);
            //check communic_point change
            _update_shedule.ProcessCommunicPointChange(shedule.shedule_id ?? -1, shedule.communic_point_id);
            //update creating date
            _update_shedule.UpdateSheduleCreatingDate(shedule.shedule_id ?? -1);
            //update periodicity
            _update_shedule.UpdatePeriodicity(shedule.shedule_id ?? -1, shedule.periodicity);
            //check status and update shedule state          
            if (shedule?.status == true)
            {
                //CreateShedule_func invoke
                return await CreateShedule(shedule);
            }
            else if (shedule?.status == false)
            {
                //DeleteShedulerFromSheduler_func invoke
                return DeleteSheduleFromSheduler(shedule.shedule_id ?? -1);
            }

            return false;
        }

        public bool DeleteSheduleFull(int shedule_id)
        {
            if (CheckSheduleExistence(shedule_id))
            {
                DeleteSheduleFromSheduler(shedule_id);
                //delete records with this shedule id from SheduleMeters table
                //delete records with this shedule id from Shedules table
                DeleteSheduleFromDb(shedule_id);
            }
            return false;
        }

        public virtual void DeleteSheduleFromDb(int shedule_id)
        {
            _database.ShedulesMeters.RemoveRange(_database.ShedulesMeters.Where(t => t.SheduleId == shedule_id).Select(t => t).ToList());
            _database.Shedules.RemoveRange(_database.Shedules.Where(t => t.Id == shedule_id).Select(t => t).ToList());
            _database.SaveChanges();
        }
        public virtual bool CheckSheduleExistence(int shedule_id)
        {
            var flag1 = _database.Shedules.Any(t => t.Id == shedule_id);
            var flag2 = _database.ShedulesMeters.Any(t => t.SheduleId == shedule_id);
            if (flag1 == true && flag2 == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual JobCreateSettings GetJobCreateSettings(SheduleCreateUpdateModel shedule, CommunicPoint? communic_point)
        {
            JobCreateSettings _job_settings = new JobCreateSettings()
            {
                job_id = shedule.shedule_id.ToString() ?? "",
                periodicity = shedule.periodicity,
                communic_point_name = communic_point?.Name ?? "",
                communic_point_port = communic_point?.Port ?? "",
                rabbit_exchange_name = _conf["DEFAULT_EXCHANGE_NAME"] ?? "",
                rabbit_server_address = _conf["RABBITMQ_SERVER_NAME"] ?? "",
                rabbit_user_name = _conf["RABBITMQ_USER_NAME"] ?? "",
                rabbit_user_password = _conf["RABBITMQ_USER_PASS"] ?? "",
                last_datetime_request = _conf["LAST_DATETIME_REQUEST"] ?? ""
            };
            return _job_settings;
        }
        public virtual List<BrokerTaskMessage> GetBrokerTaskMessageList(List<int> selected_meters_id, int shedule_id, string shedule_start_date)
        {
            var message = new List<BrokerTaskMessage>();
            foreach (int meter_id in selected_meters_id)
            {
                var meter = (from t in _database.Meters
                             where t.IdMeter == meter_id
                             select t).FirstOrDefault();

                if (meter == null)
                    throw new Exception($"Couldn`t get Meter object with MeterId={meter_id}");

                BrokerTaskMessage m = new BrokerTaskMessage()
                {
                    meter_id = meter_id,
                    shedule_id = shedule_id,
                    meter_type = meter.Type ?? "",
                    communic_interface = meter.Interface ?? "",
                    meter_address = meter.Address.ToString(),
                    sim_number = meter.Sim,
                    start_date = shedule_start_date,
                };

                message.Add(m);
            }
            return message;
        }
        public virtual CommunicPoint? GetCommunicPointInfo(int communic_point_id)
        {
            var communic_point_obj = (from t in _database.CommunicPoints
                                      where t.Id == communic_point_id
                                      select t).FirstOrDefault();

            if (communic_point_obj == null)
                throw new Exception("Не удалось получить communic_point_object по ID");

            return communic_point_obj;
        }

    }
}
