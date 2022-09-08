using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Meter
{
    public class Mercury234 : IMeterType
    {
        public string Name
        {
            get => Name;
            set => Name = "Меркурий";
        }
        public string Type
        {
            get => Type;
            set => Type = "234";
        }
    }
}
