using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class ShedulesLog
    {
        public int Id
        {
            get; set;
        }
        public string Status { get; set; } = null!;
        public DateTime DateTime
        {
            get; set;
        }
        public int SheduleId
        {
            get; set;
        }
        public string Description { get; set; } = null!;
    }
}
