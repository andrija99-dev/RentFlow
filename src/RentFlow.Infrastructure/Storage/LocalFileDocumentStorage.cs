using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentFlow.Application.Abstractions.Storage;

namespace RentFlow.Infrastructure.Storage;

/// <summary>
/// <see cref="IDocumentStorage"/> adapter that writes to the local file system. Used
/// as a fail-open fallback when no Azure Blob Storage connection string is configured,
/// so the application runs end-to-end in development without a cloud account.
/// </summary>
internal sealed class LocalFileDocumentStorage : IDocumentStorage
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileDocumentStorage> _logger;

    public LocalFileDocumentStorage(
        IOptions<BlobStorageOptions> options,
        ILogger<LocalFileDocumentStorage> logger)
    {
        var configuredPath = options.Value.LocalPath;
        var root = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(AppContext.BaseDirectory, "document-storage")
            : configuredPath;

        // Group documents under the container name so the layout mirrors the Azure adapter.
        _basePath = Path.Combine(root, options.Value.ContainerName);
        _logger = logger;
        _logger.LogInformation("Document storage: local file system at {Path}.", _basePath);
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(
        string blobName,
        byte[] content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var relativePath = blobName.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_basePath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllBytesAsync(fullPath, content, cancellationToken).ConfigureAwait(false);

        var uri = new Uri(fullPath).AbsoluteUri;
        _logger.LogInformation("Stored document {BlobName} on the local file system at {Path}.", blobName, fullPath);

        return uri;
    }
}
