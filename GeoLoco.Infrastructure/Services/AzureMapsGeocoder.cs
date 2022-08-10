using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;
using GeoLoco.Infrastructure.Model;
using Newtonsoft.Json;

namespace GeoLoco.Infrastructure.Services
{
    public class AzureMapsGeocoder : IGeocodingService
    {
        private readonly IAppConfig _config;
        private readonly IGeolocationStore _geolocationCache;
        private readonly int _millisecondDelay;
        private readonly string _key;

        public AzureMapsGeocoder(
            IAppConfig config,
            IGeolocationStore geolocationCache = null,
            string apiKey = null,
            int millisecondDelay = 40)
        {
            this._config = config;
            this._geolocationCache = geolocationCache ?? GeolocationStore.None;
            this._millisecondDelay = millisecondDelay;
            _key = apiKey ?? config.Configuration?["AzureMapsApiKey"];
            if (_key == null
                && !config.NoMoney)
            {
                throw new Exception("AzureMapsApiKey missing from environment configuration");
            }
        }

        public async Task<GeocodedAddressResponse> GeocodeAddressAsync(string fullAddress)
        {
            if (_geolocationCache.TryGet(fullAddress, out var coordinates))
            {
                _config.Log.LogVerbose($"💾 Retrieved geolocation for '{fullAddress}' from cache: {coordinates}");
                return new GeocodedAddressResponse
                {
                    FullAddress = fullAddress,
                    Coordinates = coordinates,
                    IsFromCache = true
                };
            }

            if (_config.NoMoney)
            {
                return null;
            }

            if (_millisecondDelay > 0)
            {
                // Quick and dirty way to avoid hammering the API to quickly
                await Task.Delay(_millisecondDelay);
            }

            var uri = $"https://atlas.microsoft.com/search/address/json?&subscription-key=*****&api-version=1.0&language=en-US&query={Uri.EscapeDataString(fullAddress)}";
            _config.Log.LogVerbose($"Azure Maps request '{uri}'");
            var request = new HttpRequestMessage(HttpMethod.Get, uri.Replace("*****", _key));

            var response = await _config.HttpClient
                .SendAsync(request, HttpCompletionOption.ResponseContentRead, new CancellationToken())
                .ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _config.Log.LogError($"Failed to geocode address {fullAddress}: {responseContent}");
                return null;
            }

            var azureResponse = JsonConvert.DeserializeObject<AzureMapsResponse>(responseContent);

            var resolvedAddresses = azureResponse.Results.Where(a => a.Type == "Point Address").ToList();
            if (!resolvedAddresses.Any())
            {
                _config.Log.LogWarning($"No point address found matching '{fullAddress}'. Likely does not exist. Trying range addresses for the general area.");
                resolvedAddresses = azureResponse.Results.Where(a => a.Type == "Address Range").ToList();
                if (!resolvedAddresses.Any())
                {
                    _config.Log.LogWarning($"No range addresses were found matching {fullAddress}.");
                    return null;
                }
            }

            var bestMatch = resolvedAddresses.OrderByDescending(a => a.Score).First();

            coordinates = new Coordinates(
                latitude: bestMatch.Position.Lat,
                longitude: bestMatch.Position.Lon);

            _geolocationCache.Upsert(fullAddress, coordinates);

            _config.Log.LogVerbose($"🌐 Geocoded '{fullAddress}' from Azure Maps: {coordinates}");
            return new GeocodedAddressResponse
            {
                FullAddress = fullAddress,
                Coordinates = coordinates,
            };
        }
    }
}
