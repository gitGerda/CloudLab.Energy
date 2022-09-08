namespace kzmpCloudAPI.Components.General
{
    public static class AppConsts
    {
        //EF: RabbitMQLogs table statuses
        public const string STATUS_ERROR = "error";
        public const string STATUS_WARNING = "warning";
        public const string STATUS_INFO = "info";

        //Broker messages valid types
        public enum broker_messages_types
        {
            shedule_log_type,
            power_profiles_broker_message_type,
            broker_task_message_type,
            energy_response_message_type
        };
    }
}
