using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using GuardRail.Hq.Api.Interfaces;

namespace GuardRail.Hq.Api.Implementations;

public sealed class S3RemoteFileStorage : IRemoteFileStorage
{
    private readonly AmazonS3Client _amazonS3Client;
    private readonly TransferUtility _transferUtility;

    public S3RemoteFileStorage(S3FileSettings s3FileSettings)
    {
        _amazonS3Client = new AmazonS3Client(
            new BasicAWSCredentials(s3FileSettings.AccessKey, s3FileSettings.SecretKey),
            RegionEndpoint.USEast1);
        _transferUtility = new TransferUtility(
            s3FileSettings.AccessKey,
            s3FileSettings.SecretKey,
            RegionEndpoint.USEast1);
    }

    public async Task<byte[]> GetFileAsync(
        string folder,
        string fileName,
        CancellationToken cancellationToken)
    {
        var request = new GetObjectRequest
        {
            BucketName = folder,
            Key = fileName
        };
        using var response = await _amazonS3Client.GetObjectAsync(request, cancellationToken);
        await using var responseStream = response.ResponseStream;
        var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public async Task DeleteFileAsync(
        string folder,
        string fileName,
        CancellationToken cancellationToken) =>
        await _amazonS3Client.DeleteObjectAsync(
            new DeleteObjectRequest
            {
                BucketName = folder,
                Key = fileName
            },
            cancellationToken);

    public async Task UploadFileAsync(
        string folder,
        string fileName,
        Stream data,
        CancellationToken cancellationToken) =>
        await _transferUtility.UploadAsync(
            data,
            folder,
            fileName, cancellationToken);
}