using GuardRail.Hq.Api.Interfaces;

namespace GuardRail.Hq.Api.Implementations;

public sealed class S3FileSettings : IFileSettings
{
    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string InstallFolder { get; set; }

    public string InstallFile { get; set; }
}