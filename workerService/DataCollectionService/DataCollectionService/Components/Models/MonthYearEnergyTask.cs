using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Components.Models
{
    /// <summary>
    /// Модель задачи для снятия показаний энергии
    /// </summary>
    internal class MonthYearEnergyTask
    {
        internal int month
        {
            get; set;
        }
        internal int year
        {
            get; set;
        }
    }
}
