using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GeoLoco.Core.Interfaces;

namespace GeoLoco.Core.Model.Geolocation
{
    public class GeoBoundary
    {
        public IEnumerable<Coordinates> Coordinates { get; private set; }

        public static GeoBoundary FromKml(IAppConfig config, string path)
        {
            config.Log.LogInformation($"🤺 Creating geo boundary from {path}");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var kmlContents = File.ReadAllText(path);

            // Should probably use an XML library, but this works for now.
            var match = Regex.Match(kmlContents, @"<coordinates>(?<Coords>.*?)</coordinates", RegexOptions.Singleline);

            if (!match.Success)
            {
                throw new Exception($"Unable to find geo boundary coordinates in {path}");
            }

            var textCoordinates = match.Groups["Coords"].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var coordinates = textCoordinates.Select(c => new Coordinates(c));
            var boundary = new GeoBoundary(coordinates);
            return boundary;
        }

        public GeoBoundary(IEnumerable<Coordinates> coordinates)
        {
            this.Coordinates = coordinates;

            // TODO: Verify that it is a legit polygon, or just trust KML?
        }

        public bool Encloses(Coordinates coordinates)
        {
            if (coordinates == null)
            {
                return false;
            }

            IEnumerable<IPoint> polygon = this.Coordinates;
            var point = (IPoint)coordinates;

            // Algorithm copied with 💜 from https://stackoverflow.com/a/57624683
            var intersects = new List<double>();
            var a = polygon.Last();
            foreach (var b in polygon)
            {
                if (b.X == point.X && b.Y == point.Y)
                {
                    return true;
                }

                if (b.X == a.X && point.X == a.X && point.X >= Math.Min(a.Y, b.Y) && point.Y <= Math.Max(a.Y, b.Y))
                {
                    return true;
                }

                if (b.Y == a.Y && point.Y == a.Y && point.X >= Math.Min(a.X, b.X) && point.X <= Math.Max(a.X, b.X))
                {
                    return true;
                }

                if ((b.Y < point.Y && a.Y >= point.Y) || (a.Y < point.Y && b.Y >= point.Y))
                {
                    var px = (double)(b.X + 1.0 * (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X));
                    intersects.Add(px);
                }

                a = b;
            }

            intersects.Sort();
            return intersects.IndexOf(point.X) % 2 == 0 || intersects.Count(x => x < point.X) % 2 == 1;
        }
    }
}
