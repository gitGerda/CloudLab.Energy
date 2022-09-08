using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyReportLibrary.Models
{
    public class MeterInfo
    {
        public string? meter_id
        {
            get; set;
        }
        public string? MeausuringPointNameMI
        {
            get; set;
        }
        public string? MeasuringChannelAnameMI
        {
            get; set;
        }
        public string? MeasuringChannelRnameMI
        {
            get; set;
        }
        public string? xml80020code
        {
            get; set;
        }
        public string? transformation_ratio
        {
            get; set;
        }

        public List<PowerProfileTableRec> PowerProfileTableRecList;

        public PowerProfileTableRec PowerProfileTableRecList_2330_0000;
    }
}
