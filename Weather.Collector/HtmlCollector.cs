using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Weather.Collector.Contracts;

namespace Weather.Collector
{
    internal class HtmlCollector : IHtmlCollector
    {

        ILogger _logger;

        public HtmlCollector(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> GetHtmlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response?.Content?.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }         
            }
            _logger.Log($"Fail to get html from: {url}");
            return null;
        }
    }
}
