using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Indications
{
    public class SheduleLog
    {
        public int shedule_id
        {
            get; set;
        }
        public DateTime date_time
        {
            get; set;
        }
        public string status
        {
            get; set;
        }
        public string description
        {
            get; set;
        }
    }
}
