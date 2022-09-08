using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Communicating
{
    internal class GSMReadingResponse
    {
        internal bool TimeOverFlag = false;
        internal List<byte> Data = new List<byte>();
    }
}
