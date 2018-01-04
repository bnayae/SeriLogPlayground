using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerilLogTest
{
    class Enrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var trd = propertyFactory.CreateProperty(
                            "ThreadId", Thread.CurrentThread.ManagedThreadId);
            logEvent.AddPropertyIfAbsent(trd);
            var mac = propertyFactory.CreateProperty(
                            "Machine", Environment.MachineName);
            logEvent.AddPropertyIfAbsent(mac);
        }
    }
}
