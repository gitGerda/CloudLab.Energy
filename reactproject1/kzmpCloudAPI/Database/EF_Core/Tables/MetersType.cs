using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class MetersType
    {
        public int Id
        {
            get; set;
        }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
