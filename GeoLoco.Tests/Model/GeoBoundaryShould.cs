using System.Linq;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model.Geolocation;
using GeoLoco.Infrastructure;
using GeoLoco.Tests.TestData;
using Shouldly;
using Xunit;

namespace GeoLoco.Tests.Model
{
    public class GeoBoundaryShould
    {
        private readonly IAppConfig appConfig = new AppConfig();

        [Fact]
        public void BuildFromKml()
        {
            var boundary = GeoBoundary.FromKml(appConfig, TestPaths.BoundaryKml);
            boundary.Coordinates.Count().ShouldBe(7);

            var start = boundary.Coordinates.First();
            var end = boundary.Coordinates.Last();

            start.Equals(end).ShouldBeTrue();
        }

        [Fact]
        public void KnowWhatIsInsideIt()
        {
            var boundary = GeoBoundary.FromKml(appConfig, TestPaths.BoundaryKml);

            var bearPaw = new Coordinates(latitude: 35.198864, longitude: -111.575094);
            boundary.Encloses(bearPaw).ShouldBeTrue();

            var insideWestEdge = new Coordinates(latitude: 35.194351, longitude: -111.698612);
            boundary.Encloses(insideWestEdge).ShouldBeTrue();

            var outsideWestEdge = new Coordinates(latitude: 35.194285, longitude: -111.698728);
            boundary.Encloses(outsideWestEdge).ShouldBeFalse();

            var sedona = new Coordinates(latitude: 34.869552, longitude: -111.760840);
            boundary.Encloses(sedona).ShouldBeFalse();

            var cahootsAirport = new Coordinates(latitude: 35.419938, longitude: -111.249635);
            boundary.Encloses(cahootsAirport).ShouldBeFalse();
        }
    }
}
