using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GeoLoco.Core.Model.Geolocation
{
    public class Coordinates : BaseValueObject, ICoordinates, IPoint
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public int Elevation { get; private set; }

        public double X => Latitude;
        public double Y => Longitude;

        public Coordinates(double latitude, double longitude, int elevation = 0)
        {
            if (latitude == 0 && longitude == 0)
            {
                throw new ArgumentNullException($"Coordinates of (0, 0) are probably the result of a bug. No one lives on Null Island.");
            }

            if (latitude < -90 || latitude > 90
                || longitude < -180 || longitude > 180)
            {
                throw new ArgumentOutOfRangeException($"Invalid coordinates lat:{latitude}/long:{longitude}");
            }

            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        /// <summary>
        /// Parse a string representing decimal coordinates.
        /// Assumes longitude is listed first, as in the KML spec.
        /// </summary>
        /// <param name="coordinates"></param>
        public Coordinates(string coordinates)
        {
            var match = Regex.Match(coordinates, @"^(?<Long>-?[0-9.]+),(?<Lat>-?[0-9.]+),(?<Elev>-?[0-9.]+)$");
            if (!match.Success)
            {
                throw new ArgumentException($"Invalid coordinates: {coordinates}");
            }

            Latitude = double.Parse(match.Groups["Lat"].Value);
            Longitude = double.Parse(match.Groups["Long"].Value);
            Elevation = int.Parse(match.Groups["Elev"].Value);
        }

        public override string ToString()
        {
            return $"{Latitude}, {Longitude}";
        }

        public string ToKmlString()
        {
            return $"{Longitude},{Latitude},0";
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // TODO: Because we're dealing with doubles, should probably rethink equality handling.
            yield return Latitude;
            yield return Longitude;
            yield return Elevation;
        }
    }    
}
