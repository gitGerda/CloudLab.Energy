using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireJobsToRabbitLibrary.Models
{
    public class LastIndicationHttpResponse
    {
        public string? last_date
        {
            get; set;
        }
        public string? last_time
        {
            get; set;
        }
    }
}
