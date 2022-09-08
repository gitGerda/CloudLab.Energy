using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class UsersAuth
    {
        public int UserId
        {
            get; set;
        }
        public string UserName { get; set; } = null!;
        public string UserPwd { get; set; } = null!;
        public string Salt { get; set; } = null!;
    }
}
