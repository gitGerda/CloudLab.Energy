using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Indications
{
    public class PowerProfileRecordResponse
    {
        public string? Address
        {
            get; set;
        }
        public Queue<PowerProfileRecord> Records { get; set; } = new Queue<PowerProfileRecord>();
        public Queue<Logs> Logs { get; set; } = new Queue<Logs>();
        public string? Exception;
    }

    public class PowerProfileRecord
    {
        public DateTime? RecordDate
        {
            get; set;
        }
        public double? Pplus
        {
            get; set;
        }
        public double? Pminus
        {
            get; set;
        }
        public double? Qplus
        {
            get; set;
        }
        public double? Qminus
        {
            get; set;
        }
    }
}
