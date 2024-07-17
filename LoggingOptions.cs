using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEyeLogger
{
    public class LoggingOptions
    {
        public bool LogRequestPayload { get; set; }
        public bool LogResponsePayload { get; set; }
        public bool LogHeaders { get; set; }
        public List<string> HeaderKeysToLog { get; set; } = new List<string>();
    }
}
