using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class ReportsDetalization
    {
        public int Id
        {
            get; set;
        }
        public int ReportId
        {
            get; set;
        }
        public string? DetalizationType
        {
            get; set;
        }
        public string? TblColMeterType
        {
            get; set;
        }
        public int? TblColMeterAddress
        {
            get; set;
        }
        public string? TblColTransformRatio
        {
            get; set;
        }
        public string? TblColPeriodStart
        {
            get; set;
        }
        public string? TblColPeriodEnd
        {
            get; set;
        }
        public string? TblColEnergySum
        {
            get; set;
        }
        public string? TblColPowerSum
        {
            get; set;
        }
        public string? TblColDateOfRead
        {
            get; set;
        }
        public string? TblColCheck
        {
            get; set;
        }
    }
}
