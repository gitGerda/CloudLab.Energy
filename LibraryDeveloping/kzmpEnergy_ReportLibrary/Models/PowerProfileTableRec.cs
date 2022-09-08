using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyReportLibrary.Models
{
    public class PowerProfileTableRec
    {
        public int RowNumber
        {
            get; set;
        }
        public int Id
        {
            get; set;
        }
        public int Address
        {
            get; set;
        }
        public DateTime Date
        {
            get; set;
        }
        public object Time { get; set; } = null!;
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
