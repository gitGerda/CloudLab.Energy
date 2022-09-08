using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRCLib.CRC;

namespace KzmpEnergyIndicationsLibrary.Actions.CRCCalculating
{
    public class CRC
    {
        //CRC16 algorithm: CrcAlgorithms.Crc16Modbus
        //CRC24 algorithm: CrcAlgorithms.Crc24
        public byte[] CalculateCrc(byte[] bytes, CrcAlgorithms algorithm)
        {
            CrcStdParams.StandartParameters.TryGetValue(algorithm, out Parameters crc_p);
            Crc crc = new Crc(crc_p);
            crc.Initialize();
            var crc_bytes = crc.ComputeHash(bytes);
            return crc_bytes;
        }
    }
}
