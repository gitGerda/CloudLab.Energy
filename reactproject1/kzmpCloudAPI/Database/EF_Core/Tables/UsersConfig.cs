using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class UsersConfig
    {
        public int Id
        {
            get; set;
        }
        public string UserLogin { get; set; } = null!;
        public string ConfigPropertyName { get; set; } = null!;
        public string? ConfigPropertyValue
        {
            get; set;
        }
    }
}
