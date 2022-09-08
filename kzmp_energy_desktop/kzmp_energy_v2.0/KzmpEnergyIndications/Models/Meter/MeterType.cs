using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Meter
{
    public class MeterType : IMeterType
    {
        private string _name = "";
        private string _type = "";
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public string Type
        {
            get => _type;
            set => _type = value;
        }
    }
}
