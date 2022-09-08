using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyIndicationsLibrary.Models.Indications
{
    public class SessionInitializationResponse
    {
        public Queue<Logs>? LogsQueue { get; set; } = new Queue<Logs>();
        public string? ExceptionMessage;
    }
}
