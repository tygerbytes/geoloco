using System.Threading.Tasks;
using GeoLoco.Core.Model;

namespace GeoLoco.Core.Interfaces
{
    public interface IGeocodingService
    {
        /// <summary>
        /// Geocodes an address.
        /// </summary>
        Task<GeocodedAddressResponse> GeocodeAddressAsync(string fullAddress);
    }
}
