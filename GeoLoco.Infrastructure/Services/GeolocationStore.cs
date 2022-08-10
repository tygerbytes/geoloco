using System;
using System.Collections.Generic;
using System.Linq;
using GeoLoco.Core.Interfaces;
using GeoLoco.Core.Model;
using GeoLoco.Core.Model.Geolocation;
using GeoLoco.Infrastructure.Model;
using LiteDB;
using Microsoft.Extensions.Configuration;

namespace GeoLoco.Infrastructure.Services
{
    public class GeolocationStore : IGeolocationStore
    {
        private static LiteDatabase _db;

        public static readonly IGeolocationStore None = new NullStore();

        private readonly IConfiguration _configuration;

        public GeolocationStore(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public bool TryGet(string fullAddress, out Coordinates coordinates)
        {
            var collection = GetCollection();
            var result = collection.FindOne(x => x.Key == Key(fullAddress));
            coordinates = result?.Coordinates;
            return result != null;
        }

        public void EnsureLoaded(IEnumerable<IHasGeocodedAddress> geocoded)
        {
            foreach (var geo in geocoded.Where(x => x.IsGeocoded))
            {
                Upsert(Key(geo.FullAddress), geo.Coordinates);
            }
        }

        public bool Delete(string fullAddress)
        {
            var collection = GetCollection();
            return collection.Delete(Key(fullAddress));
        }

        public void Upsert(string fullAddress, Coordinates coordinates)
        {
            if (fullAddress == null || coordinates == null)
            {
                throw new ArgumentNullException();
            }

            var key = Key(fullAddress);

            var collection = GetCollection();
            collection.Delete(key);
            collection.Insert(new AddressCoordinates(key, coordinates));
            collection.EnsureIndex(x => x.Key);
        }

        private ILiteCollection<AddressCoordinates> GetCollection()
        {
            if (_db == null)
            {
                Open();
            }

            return _db.GetCollection<AddressCoordinates>("addresses");
        }

        private void Open()
        {
            var connectionString = _configuration.GetConnectionString("GeoLocationCache");
            _db = new LiteDatabase(connectionString);
        }

        private static string Key(string fullAddress)
        {
            return fullAddress.ToLower().Trim();
        }

        private class AddressCoordinates
        {
            public AddressCoordinates()
            {
                // Don't delete. Used implicitly by LiteDB.
            }

            public AddressCoordinates(string key, Coordinates coordinates)
            {
                Key = key;
                Coordinates = coordinates;
            }

            [BsonId]
            public string Key { get; set; }
            public Coordinates Coordinates { get; set; }
        }

        private class NullStore : IGeolocationStore
        {
            public bool Delete(string fullAddress)
            {
                return true;
            }

            public void EnsureLoaded(IEnumerable<IHasGeocodedAddress> geocoded)
            {
            }

            public bool TryGet(string fullAddress, out Coordinates coordinates)
            {
                coordinates = null;
                return false;
            }

            public void Upsert(string fullAddress, Coordinates coordinates)
            {
            }
        }
    }
}
