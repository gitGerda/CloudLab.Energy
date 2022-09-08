using System;
using System.Collections.Generic;

namespace ConsoleApp2_NET
{
    public partial class PowerProfileM
    {
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public int Address { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public double? Pplus { get; set; }
        public double? Pminus { get; set; }
        public double? Qplus { get; set; }
        public double? Qminus { get; set; }
    }
}
