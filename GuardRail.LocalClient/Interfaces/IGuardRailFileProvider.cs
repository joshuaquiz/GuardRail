using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace GuardRail.LocalClient.Interfaces
{
    internal interface IGuardRailFileProvider : IFileProvider
    {
        Task AppendOrCreateAsync(
            string path,
            string text,
            CancellationToken cancellationToken);

        Task CreateOrOverride(
            string path,
            string text,
            CancellationToken cancellationToken);

        void Delete(string path);
    }
}