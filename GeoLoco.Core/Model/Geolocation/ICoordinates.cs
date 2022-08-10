namespace GeoLoco.Core.Model.Geolocation
{
    public interface ICoordinates
    {
        double Latitude { get; }
        double Longitude { get; }
        int Elevation { get; }
    }
}
