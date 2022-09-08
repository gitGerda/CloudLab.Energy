using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class FailedIndicationsTask
    {
        public int Id
        {
            get; set;
        }
        public int SheduleId
        {
            get; set;
        }
        public int MeterId
        {
            get; set;
        }
        public DateTime LastDatetime
        {
            get; set;
        }
    }
}
