using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class MsgNumber
    {
        public string CompanyInn { get; set; } = null!;
        public string? CompanyName
        {
            get; set;
        }
        public string? Contract
        {
            get; set;
        }
        public string? MsgNumber1
        {
            get; set;
        }
        public string? Date
        {
            get; set;
        }
    }
}
