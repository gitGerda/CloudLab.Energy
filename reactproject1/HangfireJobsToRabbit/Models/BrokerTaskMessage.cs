namespace HangfireJobsToRabbitLibrary.Models
{
    public class BrokerTaskMessage
    {
        public int? shedule_id
        {
            get; set;
        }
        public int? meter_id
        {
            get; set;
        }
        public string meter_type
        {
            get; set;
        }
        public string meter_address
        {
            get; set;
        }
        public string communic_interface
        {
            get; set;
        }
        public string? sim_number
        {
            get; set;
        }
        public string start_date
        {
            get; set;
        }
        public string? last_indication_datetime
        {
            get; set;
        }
    }
}
