using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Indications
{
    public class PowerProfilesBrokerMessage
    {
        public int shedule_id;
        public int meter_id;
        public Queue<PowerProfileRecord> Records { get; set; } = new Queue<PowerProfileRecord>();
    }
}
