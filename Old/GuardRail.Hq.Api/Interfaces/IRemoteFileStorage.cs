using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hq.Api.Interfaces;

public interface IRemoteFileStorage
{
    Task<byte[]> GetFileAsync(string folder, string fileName, CancellationToken cancellationToken);

    Task DeleteFileAsync(string folder, string fileName, CancellationToken cancellationToken);

    Task UploadFileAsync(string folder, string fileName, Stream data, CancellationToken cancellationToken);
}