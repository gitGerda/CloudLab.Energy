using System;
using System.Collections.Generic;

namespace ConsoleApp2_NET
{
    public partial class Meter
    {
        public int IdMeter { get; set; }
        public string? Type { get; set; }
        public int Address { get; set; }
        public string Sim { get; set; } = null!;
        public string? Inn { get; set; }
        public string? CompanyName { get; set; }
        public string? Inncenter { get; set; }
        public string? CenterName { get; set; }
        public string? MeasuringpointName { get; set; }
        public string? MeasuringchannelA { get; set; }
        public string? MeasuringchannelR { get; set; }
        public string? Xml80020code { get; set; }
        public string? TransformationRatio { get; set; }
        public string? Interface { get; set; }
    }
}
