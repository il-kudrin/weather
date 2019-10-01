using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Collector
{
    internal interface ILogger
    {
        void Log(string message);
    }
}
