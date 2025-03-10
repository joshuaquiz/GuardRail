using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using GuardRail.Core.Helpers;
using GuardRail.Hq.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuardRail.Hq.Api.Controllers;

[Route("api/install")]
public class InstallController : ControllerBase
{
    private readonly IRemoteFileStorage _remoteFileStorage;
    private readonly IFileSettings _fileSettings;

    public InstallController(
        IRemoteFileStorage remoteFileStorage,
        IFileSettings fileSettings)
    {
        _remoteFileStorage = remoteFileStorage;
        _fileSettings = fileSettings;
    }

    /// <summary>
    /// GET api/install/latest
    /// Returns the latest version payload.
    /// </summary>
    /// <returns>InstallConfiguration</returns>
    [HttpGet("latest")]
    public async Task<InstallConfiguration> GetLatestVersionAsync(CancellationToken cancellationToken)
    {
        var data = await _remoteFileStorage.GetFileAsync(
            _fileSettings.InstallFolder,
            _fileSettings.InstallFile,
            cancellationToken);
        return Encoding.UTF8
            .GetString(data, 0, data.Length)
            .FromJson<InstallConfiguration>();
    }

    /// <summary>
    /// GET api/install/update
    /// Updates the latest version payload.
    /// </summary>
    [HttpPost("update")]
    public async Task UpdateLatestVersionAsync(
        [FromBody] InstallConfiguration installConfiguration,
        CancellationToken cancellationToken) =>
        await _remoteFileStorage.UploadFileAsync(
            _fileSettings.InstallFolder,
            _fileSettings.InstallFile,
            new MemoryStream(Encoding.UTF8.GetBytes(installConfiguration.ToJson())),
            cancellationToken);
}