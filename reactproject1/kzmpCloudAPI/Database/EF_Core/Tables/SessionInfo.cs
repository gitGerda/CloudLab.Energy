using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class SessionInfo
    {
        public int UserId
        {
            get; set;
        }
        public string SessionToken { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
        public DateTime AuthTime
        {
            get; set;
        }
    }
}
