using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Collector.Contracts
{
    internal interface IHtmlCollector
    {
        Task<string> GetHtmlAsync(string url);
    }
}
