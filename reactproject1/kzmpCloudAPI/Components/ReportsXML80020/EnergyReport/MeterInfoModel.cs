using System.Text.Json.Serialization;

namespace kzmpCloudAPI.Components.ReportsXML80020.EnergyReport
{
    public class MeterInfoModel
    {
        [JsonInclude]
        public int id;
        [JsonInclude]
        public string? type;
        [JsonInclude]
        public string? address;
        [JsonInclude]
        public string? transRatio;
        [JsonInclude]
        public string? startValue;
        [JsonInclude]
        public string? endValue;
        [JsonInclude]
        public string? totalValue;
        [JsonInclude]
        public string? date;
        [JsonInclude]
        public string? powerSum;
    }
}
