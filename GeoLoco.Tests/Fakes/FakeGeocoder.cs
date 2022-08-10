using System;
using System.Threading.Tasks;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;

namespace GeoLoco.Tests.Fakes
{
    internal class FakeGeocoder : IGeocodingService
    {
        public Task<GeocodedAddressResponse> GeocodeAddressAsync(string fullAddress)
        {
            throw new NotImplementedException();
        }
    }
}
