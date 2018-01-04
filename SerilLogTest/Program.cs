using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// https://github.com/serilog/serilog/wiki/Configuration-Basics
// https://github.com/serilog/serilog/wiki/Writing-Log-Events
// https://github.com/serilog/serilog/wiki/Provided-Sinks
// https://github.com/serilog/serilog-sinks-applicationinsights
// https://github.com/serilog/serilog-sinks-elasticsearch

namespace SerilLogTest
{
    class Program
    {
        private static ConcurrentDictionary<string, LogEventLevel> _enableLog = new ConcurrentDictionary<string, LogEventLevel>
        {
            ["SerilLogTest.MyClass"] = LogEventLevel.Verbose
        };
        private const string LOG_FORMAT = "{Timestamp:HH:mm} [{Level}] (Context: {SourceContext}, Version: {Version}, Thread:{ThreadId}, Machine:{Machine}, User: {UserName}) {NewLine}{Message}{NewLine}{Exception}\r\n";
        static void Main(string[] args)
        {
            //Sample1();
            ELK();

            Console.ReadKey();

        }

        private static void Sample1()
        {
            Log.Logger = new LoggerConfiguration()
                 .Enrich.With<Enrichment>()
                 .Enrich.WithProperty("UserName", Environment.UserName)
                 .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
                 .WriteTo.Console(outputTemplate: LOG_FORMAT)
                 .Destructure.ByTransforming<Tuple<int, int, int>>(t => new { Good = t.Item1, Bad = t.Item2, Ugly = t.Item3 })
                 //.WriteTo.ColoredConsole(outputTemplate: LOG_FORMAT)
                 //.WriteTo.Console(outputTemplate: LOG_FORMAT)
                 .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Minute, outputTemplate: LOG_FORMAT)
                 .Filter.ByIncludingOnly(l =>
                 {
                     LogEventPropertyValue val;
                     if (!l.Properties.TryGetValue("SourceContext", out val))
                         return false;
                     var target = val.ToString();
                     target = target.Substring(1, target.Length - 2);
                     LogEventLevel level;
                     if (!_enableLog.TryGetValue(target, out level))
                         return false;
                     return l.Level >= level;
                 })
                 .CreateLogger();

            var my = new MyClass();
            my.Hello(1, "hi");
        }

        // https://github.com/serilog/serilog-sinks-elasticsearch
        private static void ELK()
        {
            Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console()
                 .WriteTo.Elasticsearch(
                    //new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                    new ElasticsearchSinkOptions(new Uri("http://localhost:32769")) // docker pull sebp/elk
                    {
                        IndexFormat = "demo-{0:yyyy-MM-dd}",
                        InlineFields = true,
                        //AutoRegisterTemplate = true,
                    })
                  .CreateLogger(); 
            ILogger logger = Log.Logger.ForContext<Program>();
            logger.Information("Hello, User {@User}", Environment.UserName);
            for (int i = 0; i < 200; i++)
            {
                logger.Information("Iteration {@Index} {@Data}", i, Tuple.Create( i % 10, i % 20, "X" + i % 15));
                Thread.Sleep(1000);
            }
        }
    }
}
