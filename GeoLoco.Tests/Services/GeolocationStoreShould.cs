using System;
using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;
using GeoLoco.Infrastructure;
using GeoLoco.Infrastructure.Services;
using Shouldly;
using Xunit;

namespace GeoLoco.Tests.Services
{
    public class GeolocationStoreShould
    {
        [Fact]
        public void StoreCoordinatesForAddressCrud()
        {
            var geoStore = new GeolocationStore(new AppConfig().Configuration);

            var fullAddress = "5050 Split St, Suite C, Smellington, MA 12345, United States";

            // Add it
            var coordsOriginal = new Coordinates(latitude: 53.0, longitude: 97.0);
            geoStore.Upsert(fullAddress, coordsOriginal);

            // Get it
            geoStore.TryGet(fullAddress, out var coordsRetrieved);
            coordsOriginal.Equals(coordsRetrieved).ShouldBeTrue();

            // Update it
            var coordsNew = new Coordinates(latitude: 1.0, longitude: 2.5);
            geoStore.Upsert(fullAddress, coordsNew);
            geoStore.TryGet(fullAddress, out var coordsUpdated);
            coordsUpdated.Equals(coordsNew).ShouldBeTrue();

            // Delete it
            geoStore.Delete(fullAddress);
            geoStore.TryGet(fullAddress, out var coordsDeleted);
            coordsDeleted.ShouldBeNull();
        }

        [Fact]
        public void NotStoreEmptyCoordinates()
        {
            var geoStore = new GeolocationStore(new AppConfig().Configuration);

            var fullAddress = "5050 Split St, Suite C, Smellington, MA 12345, United States";

            Should.Throw<ArgumentNullException>(
                () => geoStore.Upsert(fullAddress, null));
        }

        [Fact]
        public void IgnoreAddressCase()
        {
            var geoStore = new GeolocationStore(new AppConfig().Configuration);

            var up =  "123 MAIN ST, BUILDING 1, TOWN, MA, 12345, USA";
            var low = "123 main st, building 1, town, ma, 12345, USA";

            geoStore.Upsert(up, new Coordinates(latitude: 1.0, longitude: 1.0));
            geoStore.TryGet(low, out var retrievedCoords);
            retrievedCoords.ShouldNotBeNull();
            retrievedCoords.Latitude.ShouldBe(1.0);
            retrievedCoords.Longitude.ShouldBe(1.0);
        }
    }
}
