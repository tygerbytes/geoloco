using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;
using CsvHelper;
using CsvHelper.Configuration;

namespace GeoLoco.Core.Services
{
    public class SimpleCsvParser : ICheckInputFile
    {
        private readonly IAppConfig config;

        public SimpleCsvParser(IAppConfig config)
        {
            this.config = config;
        }

        public async Task<int> CheckAsync(string path, CancellationToken cancellationToken)
        {
            var log = config.Log;
            log.LogInformation($"🔍 Checking '{path}' for errors");

            ValidateFile(path);

            await using var fs = File.OpenRead(path);
            using var s = new StreamReader(fs);
            using var csv = new CsvReader(s, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<SimpleCsvAddressReaderWriterMap>();

            var errorCount = 0;
            var lineNumber = 0;
            while (await csv.ReadAsync())
            {
                lineNumber++;

                if (cancellationToken.IsCancellationRequested)
                {
                    config.Log.LogWarning("Operation cancelled");
                    return -1;
                }

                SimpleCsvAddress addy;
                try
                {
                    addy = csv.GetRecord<SimpleCsvAddress>();
                }
                catch (Exception ex)
                {
                    log.LogError($"Error reading simple record at line {lineNumber}: {ex.Message}");
                    errorCount++;
                    continue;
                }

                log.LogVerbose($"📩→{lineNumber}: {addy}");
            }

            log.LogInformation($"🏁 Finished checking {path}. There were {errorCount} errors.");

            return errorCount;
        }

        public async Task<IEnumerable<SimpleCsvAddress>> ParseAsync(StreamReader reader, CancellationToken cancellationToken = default)
        {
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<SimpleCsvAddressReaderWriterMap>();
            var recordsAsync = csv.GetRecordsAsync<SimpleCsvAddress>();

            var addresses = new List<SimpleCsvAddress>();
            await foreach (var record in recordsAsync.WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                addresses.Add(record);
            }
            return addresses;
        }

        public async Task<IEnumerable<SimpleCsvAddress>> ParseAsync(string path, CancellationToken cancellationToken = default)
        {
            config.Log.LogInformation($"🔃 Loading addresses from '{path}'");

            ValidateFile(path);

            await using var fs = File.OpenRead(path);
            using var s = new StreamReader(fs);
            return await ParseAsync(s, cancellationToken);
        }

        public async Task WriteToCsvAsync(StreamWriter writer, IEnumerable<SimpleCsvAddress> addresses)
        {
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<SimpleCsvAddressReaderWriterMap>();
            await csv.WriteRecordsAsync(addresses);
        }

        public async Task WriteToCsvAsync(string path, IEnumerable<SimpleCsvAddress> addresses)
        {
            config.Log.LogInformation($"📝 Exporting {addresses.Count()} homes to {path}.");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await using var fs = File.OpenWrite(path);
            var s = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true /* Make Excel happy */));

            using (s)
            {
                await WriteToCsvAsync(s, addresses);
            }
        }

        public async Task CreateTemplateFileAsync(string outputFileName)
        {
            var addresses = new List<SimpleCsvAddress>
            {
                new()
                {
                    Label = "Trader Joe's Portland - Hollywood (144)",
                    FullAddress = "4121 NE Halsey St, Portland, OR 97232 US",
                },
                new()
                {
                    Label = "Trader Joe's Portland Nw (146)",
                    FullAddress = "2122 NW Glisan St, Portland, OR 97210 US",
                    Latitude = 45.52843140640777,
                    Longitude = -122.69446666024372,
                }
            };

            await WriteToCsvAsync(outputFileName, addresses);
        }

        private void ValidateFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"No file with path {path}");
            }
        }

        protected sealed class SimpleCsvAddressReaderWriterMap : ClassMap<SimpleCsvAddress>
        {
            public SimpleCsvAddressReaderWriterMap()
            {
                Map(m => m.Label).Name("Label");
                Map(m => m.FullAddress).Name("FullAddress");
                Map(m => m.Latitude).Name("Latitude");
                Map(m => m.Longitude).Name("Longitude");
            }
        }
    }
}
