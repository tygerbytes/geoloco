using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;
using GeoLoco.Core.Services;
using GeoLoco.Infrastructure;
using GeoLoco.Infrastructure.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("GeoLoco.Tests")]

namespace GeoLoco.CLI
{
    [ValidateRequiredFiles]
    [Command(Name = "geoloco", Description = "Takes an address list (*.csv), geocodes them with Azure Maps, and produces a new .csv or .kml file")]
    [HelpOption("-h")]
    public class Program
    {
        private readonly IAppConfig appConfig;
        private readonly IGeolocationStore geoStore;
        private readonly SimpleCsvParser simpleCsvParser;
        private readonly AzureMapsGeocoder geoCoder;

        public static async Task<int> Main(string[] args)
        {
            var quiet = args.Contains("--quiet");
            var noMoney = args.Contains("--no-money");

            var appConfig = new AppConfig(noMoney: noMoney, quiet: quiet);

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(ConfigureServices(appConfig));

            var returnCode = await app.ExecuteAsync(args);
            return returnCode;
        }

        public Program(
            IAppConfig appConfig,
            IGeolocationStore geoStore,
            SimpleCsvParser simpleCsvParser,
            AzureMapsGeocoder geoCoder)
        {
            this.appConfig = appConfig;
            this.geoStore = geoStore;
            this.simpleCsvParser = simpleCsvParser;
            this.geoCoder = geoCoder;
        }

        [Option(Description = "Path to .csv file to geocode", ShortName = "f")]
        public string AddressListCsv { get; }

        [Option(Description = "Path to write output to. (.csv|.kml) extension. (Will be overwritten.)", ShortName = "o")]
        public string OutputFile { get; }

        [Option(Description = "Path to file (*.kml) containing boundaries to include in kml output.", ShortName = "b")]
        public string BoundaryKml { get; }

        [Option(Description = "Don't use any services like Azure Maps that could cost $$$.", ShortName = "nm")]
        public bool NoMoney { get; } = false;

        [Option(Description = "Check the address list .csv file for errors.", ShortName = "ck")]
        public bool Check { get; } = false;

        [Option(Description = "Writes a CSV template to address_template.csv", ShortName = "gt")]
        public bool GenerateCsvTemplate { get; } = false;

        [Option(Description = "Path to log file. (Will be overwritten.)", ShortName = "l")]
        public string LogPath { get; }

        [Option(Description = "Log verbose output to the console (--log-path is always verbose)", ShortName = "v")]
        public bool Verbose { get; } = false;

        [Option(Description = "Don't log anything to the console.")]
        public bool Quiet { get; } = false;

        [Option(Description = "Print program version.", LongName = "version", ShortName = "vs")]
        public bool CheckVersion { get; } = false;

        public async Task<int> RunAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            ConfigureLoggers((AppConfig)appConfig);

            if (CheckVersion)
            {
                appConfig.Log.LogInformation($"GeoLoco version {appConfig.Version}");
                return 0;
            }

            if (GenerateCsvTemplate)
            {
                await simpleCsvParser.CreateTemplateFileAsync("./address_template.csv");
            }

            if (Check)
            {
                if (AddressListCsv != null)
                {
                    return await simpleCsvParser.CheckAsync(path: AddressListCsv, cancellationToken: cancellationToken);
                }

                return 0;
            }

            var sessionStart = appConfig.Date.Now;

            if (AddressListCsv == null)
            {
                // Nothing else to do
                return 0;
            }

            appConfig.Log.LogInformation($"Reading {AddressListCsv}");
            var addresses = (await simpleCsvParser.ParseAsync(
                    path: AddressListCsv,
                    cancellationToken: cancellationToken))
                .ToList();

            appConfig.Log.LogInformation($"Priming geolocation cache.");
            geoStore.EnsureLoaded(addresses.Where(h => h.IsGeocoded));

            appConfig.Log.LogInformation("Init Azure Maps Geocoder");
            if (appConfig.NoMoney)
            {
                appConfig.Log.LogWarning($"Option 'NoMoney' detected. Will avoid geocoding missing address coordinates.");
            }

            foreach (var record in addresses)
            {
                if (record.Coordinates == null)
                {
                    var result = await geoCoder.GeocodeAddressAsync(record.FullAddress);
                    if (result != null)
                    {
                        record.Latitude = result.Coordinates.Latitude;
                        record.Longitude = result.Coordinates.Longitude;
                    }
                }
            }

            var ext = OutputFile.ToLower().Split(".").Last();
            if (ext == "csv")
            {
                await simpleCsvParser.WriteToCsvAsync(OutputFile, addresses);
            }
            else if (ext == "kml")
            {
                GeoBoundary boundary = null;
                if (BoundaryKml != null)
                {
                    boundary = GeoBoundary.FromKml(appConfig, BoundaryKml);
                }

                var points = addresses.Select(a => new PointLocation { Label = a.Label, Coordinates = a.Coordinates });

                var kmlBuilder = new KmlBuilder()
                    .AddBoundary(boundary)
                    .AddPointLocations(points);

                var kml = kmlBuilder.Build();

                await File.WriteAllTextAsync(OutputFile, kml, cancellationToken);
            }

            appConfig.Log.LogInformation("🏁 All done.");
            return 0;
        }

        internal static ServiceProvider ConfigureServices(IAppConfig appConfig)
        {
            var geoStore = new GeolocationStore(appConfig.Configuration);

            var services = new ServiceCollection()
                .AddSingleton<IAppConfig>(appConfig)
                .AddSingleton<IGeolocationStore>(geoStore)
                .AddSingleton(new SimpleCsvParser(appConfig))
                .AddSingleton(new AzureMapsGeocoder(appConfig, geoStore))
                .BuildServiceProvider();

            return services;
        }

        private void ConfigureLoggers(AppConfig config)
        {
            var loggers = new List<IAppLogger>();
            if (!Quiet)
            {
                loggers.Add(new ConsoleLogger(verbose: Verbose));
            }

            if (LogPath != null)
            {
                loggers.Add(new FileLogger(LogPath, config.Date, verbose: true));
            }

            config.Log = new AggregateLogger(loggers);
        }

        /// <summary>
        /// Called by CommandLineApplication
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            return await RunAsync(app, cancellationToken);
        }
    }
}
