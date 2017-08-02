﻿using Orleans.Runtime.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace Orleans.AspNetCore
{
    public class OrleansHostBuilder : IOrleansHostBuilder
    {
        public IOrleansHost Build()
        {
            var host = new OrleansHost();

            var config = host.Config;
            config.Defaults.SiloName = "default";
            config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.MembershipTableGrain;
            config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.ReminderTableGrain;
            config.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, 30000);
            config.Defaults.Port = 11111;
            config.Globals.RegisterStorageProvider("Orleans.Storage.MemoryStorage", "Default");
            //config.Globals.RegisterBootstrapProvider("OrleansDashboard.Dashboard", "Dashboard");
            if (IsLinux())
            {
                var hostName = Dns.GetHostName();
                var ips = Dns.GetHostAddressesAsync(hostName).Result;
                config.Defaults.HostNameOrIPAddress = hostName;
                config.Globals.SeedNodes.Add(new IPEndPoint(ips.First(), 11111));
                config.PrimaryNode = config.Globals.SeedNodes.First();
            }
            else
            {
                config.Defaults.HostNameOrIPAddress = "localhost";
                config.Globals.SeedNodes.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111));
                config.PrimaryNode = config.Globals.SeedNodes.First();
            }

            return host;
        }

        private bool IsLinux()
        {
            // TODO: Improve. Stupid solution. Need to check if in container.
            return RuntimeInformation.OSDescription.Contains("Linux");
        }

        public IOrleansHostBuilder UseStartup<TStartup>() where TStartup : class
        {
            return this;
        }
    }
}