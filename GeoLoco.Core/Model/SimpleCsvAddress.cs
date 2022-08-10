using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model.Geolocation;

namespace GeoLoco.Core.Model
{
    public class SimpleCsvAddress : IHasGeocodedAddress
    {
        public string Label { get; set; }
        public string FullAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public Coordinates Coordinates
        {
            get
            {
                if (Latitude.HasValue && Longitude.HasValue)
                {
                    return new Coordinates(
                        latitude: Latitude.Value,
                        longitude: Longitude.Value);
                }

                return null;
            }
        }

        public bool IsGeocoded
        {
            get
            {
                return Coordinates != null;
            }
        }

        public override string ToString()
        {
            return FullAddress;
        }
    }
}
