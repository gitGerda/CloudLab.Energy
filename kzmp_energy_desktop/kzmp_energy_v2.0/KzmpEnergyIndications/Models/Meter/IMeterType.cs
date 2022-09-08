using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Meter
{
    public interface IMeterType
    {
        string Name
        {
            get; set;
        }

        string Type
        {
            get; set;
        }
    }
}
