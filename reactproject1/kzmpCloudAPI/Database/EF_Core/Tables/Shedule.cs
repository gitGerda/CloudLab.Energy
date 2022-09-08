using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class Shedule
    {
        public int Id
        {
            get; set;
        }
        public string Name { get; set; } = null!;
        public bool Status
        {
            get; set;
        }
        public DateTime CreatingDate
        {
            get; set;
        }
        public int CommunicPointId
        {
            get; set;
        }
        public string Shedule1 { get; set; } = null!;
    }
}
