namespace kzmpCloudAPI.Controllers.Meters
{
    public class MetersByCompanyINN
    {
        public string? companyInn
        {
            get; set;
        }
        public string? companyName
        {
            get; set;
        }
        public string? centerInn
        {
            get; set;
        }
        public string? centerName
        {
            get; set;
        }
        public string? contractNumber
        {
            get; set;
        }
        public string? xml80020MsgNumber
        {
            get; set;
        }
        public string? xml80020MsgNumberDate
        {
            get; set;
        }

        public List<MeterInfoBySim>? meters
        {
            get; set;
        }
    }

    public class MeterInfoBySim
    {
        public string? sim
        {
            get; set;
        }
        public List<MeterInfo>? metersINFO
        {
            get; set;
        }

    }
}
