using System;
using System.Collections.Generic;

namespace kzmpCloudAPI.Database.EF_Core.Tables
{
    public partial class RabbitMQLog
    {
        public int Id
        {
            get; set;
        }
        public DateTime Date
        {
            get; set;
        }
        public string? Status
        {
            get; set;
        }
        public string? Message
        {
            get; set;
        }
    }
}
