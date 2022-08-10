using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;
using GeoLoco.Core.Services;
using GeoLoco.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace GeoLoco.Infrastructure
{
    public class AppConfig : IAppConfig
    {
        public string Version { get; }  = "1.0.0";

        public AppConfig(
            IConfiguration configuration = null,
            IAppLogger appLogger = null,
            bool noMoney = false,
            bool quiet = false)
        {
            if (configuration == null)
            {
                configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            }

            Configuration = configuration;
            NoMoney = noMoney;

            Log = appLogger;
            if (!quiet)
            {
                Log = new ConsoleLogger(verbose: false);
            }
        }

        public IAppLogger Log { get; set; }

        public bool NoMoney { get; }

        public HttpClient HttpClient => new HttpClient();

        public IConfiguration Configuration { get; }

        public IDateService Date => new DateService();
    }
}
