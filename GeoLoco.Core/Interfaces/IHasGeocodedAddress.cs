using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;

namespace GeoLoco.Core.Interfaces
{
    public interface IHasGeocodedAddress
    {
        string FullAddress { get; }
        Coordinates Coordinates { get; }

        bool IsGeocoded { get; }
    }
}
