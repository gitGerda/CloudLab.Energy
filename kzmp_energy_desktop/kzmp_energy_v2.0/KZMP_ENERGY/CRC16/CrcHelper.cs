using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZMP_ENERGY.CRC16
{
    internal static class CrcHelper
    {
        internal static ulong ReverseBits(ulong ul, int valueLength)
        {
            ulong newValue = 0;

            for (int i = valueLength - 1; i >= 0; i--)
            {
                newValue |= (ul & 1) << i;
                ul >>= 1;
            }

            return newValue;
        }
    }
}
