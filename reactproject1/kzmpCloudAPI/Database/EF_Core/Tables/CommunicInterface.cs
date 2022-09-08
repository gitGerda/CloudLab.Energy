using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class CommunicInterface
    {
        public int Id
        {
            get; set;
        }
        public string Name { get; set; } = null!;
    }
}
