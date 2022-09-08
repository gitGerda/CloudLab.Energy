using System;
using System.Collections.Generic;

namespace ConsoleApp2_NET
{
    public partial class EnergyTable
    {
        public int RowNumber { get; set; }
        public int MeterId { get; set; }
        public int? Address { get; set; }
        public int Year { get; set; }
        public string Month { get; set; } = null!;
        public string? StartValue { get; set; }
        public string? EndValue { get; set; }
        public string? Total { get; set; }
        public string? Date { get; set; }
    }
}
