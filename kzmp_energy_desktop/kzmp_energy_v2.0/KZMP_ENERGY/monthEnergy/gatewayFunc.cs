using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KZMP_ENERGY.monthEnergy
{
    public static class gatewayFunc
    {
 
        public static void gatewayWrite(byte [] power, ref SerialPort port)
        {
            byte[] power_gateway = new byte[15];

            //num
            power_gateway[3] = 0x01;
            power_gateway[4] = 0x00;
            //len
            power_gateway[5] = 0x06;
            power_gateway[6] = 0x00;
            //port
            power_gateway[7] = 0x01;
            //payload
            power_gateway[8] = power[0];
            power_gateway[9] = power[1];
            power_gateway[10] = power[2];
            power_gateway[11] = power[3];
            power_gateway[12] = power[4];
            power_gateway[13] = power[5];

            //checksum
            power_gateway[14] = Convert.ToByte((power_gateway[8] + power_gateway[9] + power_gateway[10] + power_gateway[11]
                + power_gateway[12] + power_gateway[13] + 0xff) & 0xff);

            //crc24
            byte[] local_mas = new byte[] { power_gateway[3], power_gateway[4], power_gateway[5], power_gateway[6], power_gateway[7] };
            byte[] crc24 = FormPowerProfile.CalculateCrc24(local_mas);
            power_gateway[0] = crc24[0];
            power_gateway[1] = crc24[1];
            power_gateway[2] = crc24[2];

            if (!port.CDHolding)
            {
                MessageBox.Show(
                "Ошибка связи: потеря несущей.\nБудет выполнена перезагрузка программы.",
                "kzmp_energy notification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

                Application.Restart();
            }

            port.DiscardInBuffer();
            port.Write(power_gateway, 0, power_gateway.Length);

        }
    }
}
