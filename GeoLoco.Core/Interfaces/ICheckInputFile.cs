using System.Threading;
using System.Threading.Tasks;

namespace GeoLoco.Core.Interfaces
{
    public interface ICheckInputFile
    {
        Task<int> CheckAsync(string path, CancellationToken cancellationToken);
    }
}
