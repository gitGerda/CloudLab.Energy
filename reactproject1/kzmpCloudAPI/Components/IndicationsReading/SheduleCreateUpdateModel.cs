namespace kzmpCloudAPI.Components.IndicationsReading
{
    public class SheduleCreateUpdateModel
    {
        public int? shedule_id
        {
            get; set;
        }
        public string shedule_name
        {
            get; set;
        } = "";
        public int communic_point_id
        {
            get; set;
        }
        public List<int> selected_meters_id
        {
            get; set;
        }
        public string periodicity
        {
            get; set;
        } = "";
        public bool? status
        {
            get; set;
        }
        public string? start_date
        {
            get; set;
        }
    }
}