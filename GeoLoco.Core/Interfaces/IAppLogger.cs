namespace GeoLoco.Core.Interfaces
{
    public interface IAppLogger
    {
        void LogInformation(string message);
        void LogVerbose(string message);

        void LogWarning(string message);

        void LogError(string message);
    }
}
