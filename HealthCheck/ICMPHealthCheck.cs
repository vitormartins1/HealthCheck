using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private readonly string Host;
        private readonly int HealthyRoundTripTime;

        public ICMPHealthCheck(string host, int healthyRoundTripTime)
        {
            Host = host;
            HealthyRoundTripTime = healthyRoundTripTime;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(Host);

                switch (reply.Status)
                {
                    case IPStatus.Success:
                        string msg = $"ICMP to {Host} took {reply.RoundtripTime} ms.";
                        return (reply.RoundtripTime > HealthyRoundTripTime) 
                            ? HealthCheckResult.Degraded(msg) 
                            : HealthCheckResult.Healthy(msg);
                    default:
                        string err = $"ICMP to {Host} failed: {reply.Status}";
                        return HealthCheckResult.Unhealthy(err);
                }
            }
            catch (Exception e)
            {
                string err = $"ICMP to {Host} failed: {e.Message}";
                return HealthCheckResult.Unhealthy(err);
            }
        }
    }
}