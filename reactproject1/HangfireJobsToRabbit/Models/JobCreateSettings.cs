namespace HangfireJobsToRabbitLibrary.Models
{
    public class JobCreateSettings
    {
        public string job_id
        {
            get; set;
        }
        public string periodicity
        {
            get; set;
        }

        public string last_datetime_request
        {
            get; set;
        }
        public string communic_point_name
        {
            get; set;
        }
        public string communic_point_port
        {
            get; set;
        }
        public string rabbit_exchange_name
        {
            get; set;
        }
        public string rabbit_server_address
        {
            get; set;
        }
        public string rabbit_user_name
        {
            get; set;
        }
        public string rabbit_user_password
        {
            get; set;
        }
    }
}
