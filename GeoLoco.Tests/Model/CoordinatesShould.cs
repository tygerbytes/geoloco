using System;
using GeoLoco.Core.Model.Geolocation;
using Shouldly;
using Xunit;

namespace GeoLoco.Tests.Model
{
    public class CoordinatesShould
    {
        [Fact]
        public void ParseFromString()
        {
            var coords = new Coordinates("-122.2378845,45.70126659999999,1");
            coords.Latitude.ShouldBe(45.7012665999999, .0001);
            coords.Longitude.ShouldBe(-122.2378845, .0001);
            coords.Elevation.ShouldBe(1);
        }

        [Fact]
        public void BlowUpForInvalidCoordinates()
        {
            Should.Throw<ArgumentOutOfRangeException>(
                () => new Coordinates(-181, 91));
        }

        [Fact]
        public void BlowUpForZeroCoordinates()
        {
            // No one really lives on Null Island :-D
            Should.Throw<ArgumentNullException>(
                () => new Coordinates(0, 0));
        }
    }
}
