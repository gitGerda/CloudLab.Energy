using System;
using System.Collections.Generic;

namespace ConsoleApp2_NET
{
    public partial class ReportsHistory
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public DateTime ReportsDate { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public string CompanyInn { get; set; } = null!;
    }
}
