using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class CommunicPoint
    {
        public int Id
        {
            get; set;
        }
        public string Name { get; set; } = null!;
        public string Port { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
