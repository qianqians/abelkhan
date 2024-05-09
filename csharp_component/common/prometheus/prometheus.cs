using Prometheus;
using System.Collections.Generic;
using System.Net;

namespace Service
{
    public class PrometheusMetric
    {
        private MetricServer serverPrometheusMetric;

        private Dictionary<string, Counter> metricCounters;

        public PrometheusMetric(short port)
        {
            serverPrometheusMetric = new MetricServer(port);
        }

        public void Start()
        {
            try
            {
                serverPrometheusMetric.Start();
            }
            catch (HttpListenerException ex)
            {
                Log.Log.err($"Failed to start metric server: {ex.Message}");
                Log.Log.err("You may need to grant permissions to your user account if not running as Administrator:");
                Log.Log.err("netsh http add urlacl url=http://+:1234/metrics user=DOMAIN\\user");
            }
        }

        public Counter GetCreateCounter(string name)
        {
            if (metricCounters.TryGetValue(name, out var counter))
            {
                return counter;
            }

            counter = Metrics.CreateCounter(name, "prometheus metric user counter");
            metricCounters.Add(name, counter);

            return counter;
        }
    }
}