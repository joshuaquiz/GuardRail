using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.LocalClient.Interfaces;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace GuardRail.LocalClient.Implementations;

internal sealed class GuardRailFileProvider : PhysicalFileProvider, IGuardRailFileProvider
{
    public GuardRailFileProvider(string root) : base(root)
    {
    }

    public GuardRailFileProvider(string root, ExclusionFilters filters)
        : base(root, filters)
    {
    }

    public async Task AppendOrCreateAsync(
        string path,
        string text,
        CancellationToken cancellationToken) =>
        await File.AppendAllTextAsync(path, text, cancellationToken);

    public async Task CreateOrOverride(
        string path,
        string text,
        CancellationToken cancellationToken) =>
        await File.WriteAllTextAsync(path, text, cancellationToken);

    public void Delete(string path) =>
        File.Delete(path);
}