using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRCLib.CRC
{
    public static class CrcTest
    {
        public static void Test()
        {
            foreach (var p in CrcStdParams.StandartParameters.Values)
            {
                var crc = new Crc(p);
                if (!crc.IsRight())
                {
                    Console.WriteLine(p.Name);
                }
            }
        }
    }
}
