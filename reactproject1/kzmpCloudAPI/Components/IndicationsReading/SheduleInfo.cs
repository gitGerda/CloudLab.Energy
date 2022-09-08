namespace kzmpCloudAPI.Components.IndicationsReading
{
    public class SheduleInfo
    {
        public int shedule_id
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public bool status
        {
            get; set;
        }
        public DateTime creating_date
        {
            get; set;
        }

        public string shedule
        {
            get; set;
        }
        public int communicPointId
        {
            get; set;
        }
        public string communicPointName
        {
            get; set;
        }
        public int countRemoteModems
        {
            get; set;
        }
        public int countRemoteMeters
        {
            get; set;
        }
        public DateTime? lastReadingDate
        {
            get; set;
        }
    }
}
