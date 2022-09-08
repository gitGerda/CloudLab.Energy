namespace kzmpCloudAPI.Components.IndicationsReading
{
    public class SheduleSettingsMeters
    {
        public int meter_id
        {
            get; set;
        }
        public string type
        {
            get; set;
        }
        public int address
        {
            get; set;
        }
        public string company
        {
            get; set;
        }
        public string xml80020
        {
            get; set;
        }
        public string communic_interface
        {
            get; set;
        }
        public string sim_number
        {
            get; set;
        }
        public bool select_flag
        {
            get; set;
        } = false;
    }
}
