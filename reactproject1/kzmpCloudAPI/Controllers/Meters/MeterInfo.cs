namespace kzmpCloudAPI.Controllers.Meters
{
    public class MeterInfo
    {
        public string? id_meter
        {
            get; set;
        }
        public string? type
        {
            get; set;
        }
        public string? address
        {
            get; set;
        }
        public string? sim
        {
            get; set;
        }
        public string? Interface
        {
            get; set;
        }
        public DateTime? lastIndicationDate
        {
            get;
            set;
        }
        public TimeSpan? lastIndicationTime
        {
            get;
            set;
        }
    }
}
