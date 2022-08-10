using GeoLoco.Core.Model.Geolocation;

namespace GeoLoco.Core.Model
{
    public class GeocodedAddressResponse
    {
        public string FullAddress { get; set; }
        public Coordinates Coordinates { get; set; }
        public bool IsFromCache { get; set; }
    }
}
