using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Devices
{
    internal static class DevicesCommon
    {
        internal static Queue<T>? JoinTwoQueuesHook<T>(Queue<T>? sourse, Queue<T>? receiver)
        {
            if (sourse != null && sourse.Count > 0 && receiver != null)
            {
                foreach (var el in sourse)
                {
                    receiver.Enqueue(el);
                }
                return receiver;
            }
            else
            {
                return null;
            }
        }
    }
}
