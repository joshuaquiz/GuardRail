using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace GuardRail.LocalClient.Interfaces;

/// <summary>
/// An interface for abstracting getting files.
/// </summary>
public interface IGuardRailFileProvider : IFileProvider
{
    /// <summary>
    /// Append to or create a new file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="text">The text to put into the file.</param>
    /// <param name="cancellationToken">A CancellationToken.</param>
    /// <returns>Task</returns>
    Task AppendOrCreateAsync(
        string path,
        string text,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new or overrides an existing file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="text">The text to put into the file.</param>
    /// <param name="cancellationToken">A CancellationToken.</param>
    /// <returns>Task</returns>
    Task CreateOrOverride(
        string path,
        string text,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    void Delete(string path);
}