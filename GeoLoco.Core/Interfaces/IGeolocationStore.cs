using System.Collections.Generic;
using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;

namespace GeoLoco.Core.Interfaces
{
    public interface IGeolocationStore
    {
        public bool TryGet(string fullAddress, out Coordinates coordinates);

        void Upsert(string fullAddress, Coordinates coordinates);

        bool Delete(string fullAddress);

        void EnsureLoaded(IEnumerable<IHasGeocodedAddress> geocoded);
    }
}
