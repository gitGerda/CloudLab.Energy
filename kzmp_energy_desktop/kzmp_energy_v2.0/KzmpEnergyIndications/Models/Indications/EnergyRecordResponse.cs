using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Indications
{
    public class EnergyRecordResponse
    {
        public int? meter_id;
        public int? shedule_id;
        public int MeterAddress
        {
            get; set;
        }
        public int Year
        {
            get; set;
        }
        public int MonthNumber
        {
            get; set;
        }
        public double StartValue
        {
            get; set;
        }
        public double EndValue
        {
            get; set;
        }
        public double TotalValue
        {
            get; set;
        }
        public Queue<Logs> Logs
        {
            get; set;
        } = new Queue<Logs>();
    }
}
