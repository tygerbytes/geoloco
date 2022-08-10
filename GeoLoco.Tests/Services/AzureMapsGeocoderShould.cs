using System.Threading.Tasks;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;
using GeoLoco.Infrastructure;
using GeoLoco.Infrastructure.Services;
using Shouldly;
using Xunit;

namespace GeoLoco.Tests.Services
{
    public class AzureMapsGeocoderShould
    {
        [Fact]
        public async Task GeocodeAnAddress()
        {
            var appConfig = new AppConfig();

            var geocoder = new AzureMapsGeocoder(appConfig, millisecondDelay: 0);

            const string address = "400 Broad St, Seattle WA 98109, United States";

            var coordinates = (await geocoder.GeocodeAddressAsync(address)).Coordinates;

            coordinates.ShouldNotBeNull();
            coordinates.Latitude.ShouldBe(47.6204, .001);
            coordinates.Longitude.ShouldBe(-122.3491, .001);
        }

        [Fact]
        public async Task GeocodeGeneralLocationOfFakeAddress()
        {
            var appConfig = new AppConfig();

            var geocoder = new AzureMapsGeocoder(appConfig, millisecondDelay: 0);

            // Fake address right next to Space Needle
            const string address = "400 Broad St, Seattle WA 98109, United States";

            var response = await geocoder.GeocodeAddressAsync(address);

            // We can still geocode the general location
            response.Coordinates.ShouldNotBeNull();
            response.Coordinates.Latitude.ShouldBe(47.61975, .001);
            response.Coordinates.Longitude.ShouldBe(-122.34868, .001);
        }

        [Fact]
        public async Task GetLocationFromCache()
        {
            var appConfig = new AppConfig();

            IGeolocationStore geocache = new GeolocationStore(appConfig.Configuration);

            const string address = "400 Broad St, Seattle WA 98109, United States";

            geocache.Upsert(address, new Coordinates(latitude: 47.6204, longitude: -122.3491));

            var geocoder = new AzureMapsGeocoder(appConfig, geolocationCache: geocache, apiKey: "NONE", millisecondDelay: 0);

            var coordinates = (await geocoder.GeocodeAddressAsync(address)).Coordinates;
            coordinates.ShouldNotBeNull();
            coordinates.Latitude.ShouldBe(47.6204, .001);
            coordinates.Longitude.ShouldBe(-122.3491, .001);
        }
    }
}
