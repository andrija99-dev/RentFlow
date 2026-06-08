using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentFlow.Application.Abstractions.Storage;

namespace RentFlow.Infrastructure.Storage;

/// <summary>
/// <see cref="IDocumentStorage"/> adapter backed by Azure Blob Storage. Used when a
/// blob storage connection string is configured.
/// </summary>
internal sealed class AzureBlobDocumentStorage : IDocumentStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageOptions _options;

    public AzureBlobDocumentStorage(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> options,
        ILogger<AzureBlobDocumentStorage> logger)
    {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
        logger.LogInformation(
            "Document storage: Azure Blob ({Endpoint}, container '{Container}').",
            blobServiceClient.Uri,
            _options.ContainerName);
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(
        string blobName,
        byte[] content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var blob = container.GetBlobClient(blobName);

        using var stream = new MemoryStream(content);
        await blob.UploadAsync(
            stream,
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } },
            cancellationToken).ConfigureAwait(false);

        return blob.Uri.AbsoluteUri;
    }
}
