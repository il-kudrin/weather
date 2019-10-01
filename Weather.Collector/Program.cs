
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weather.Data;

namespace Weather.Collector
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();
            var collector = new HtmlCollector(logger);
            var parser = new GisMeteoParser(collector, logger);
            var sheduler = new WeatherScheduler(parser, logger);
            using (var cts = new CancellationTokenSource())
            {
                Console.WriteLine("started: press any key to stop.");
                var shedulerTask =Task.Run(() => sheduler.Run(cts.Token).GetAwaiter().GetResult());
                Console.ReadKey();
                Console.WriteLine("Stopping... Wait current iteration to complite.");
                cts.Cancel();
                shedulerTask.Wait();
            }
        }

    }
}
