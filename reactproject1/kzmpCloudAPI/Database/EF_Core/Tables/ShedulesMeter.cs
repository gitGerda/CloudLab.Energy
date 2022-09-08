using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class ShedulesMeter
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
    }
}
