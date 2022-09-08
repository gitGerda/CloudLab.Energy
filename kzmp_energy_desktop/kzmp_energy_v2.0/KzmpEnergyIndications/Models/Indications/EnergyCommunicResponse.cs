using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Indications
{
    internal class EnergyCommunicResponse
    {
        internal double Value
        {
            get; set;
        }
        internal Queue<Logs> Logs
        {
            get; set;
        } = new Queue<Logs>();
    }
}
