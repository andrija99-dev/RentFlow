namespace RentFlow.Application.Abstractions.Storage;

/// <summary>
/// Adapter over the document blob store. Wraps the external storage provider
/// (Azure Blob Storage in production, the local file system in development) so the
/// rest of the application stores and references generated documents uniformly.
/// </summary>
public interface IDocumentStorage
{
    /// <summary>Uploads a document and returns a durable locator for it.</summary>
    /// <param name="blobName">The blob path/name, for example <c>contracts/{id}.html</c>.</param>
    /// <param name="content">The document bytes.</param>
    /// <param name="contentType">The MIME content type, for example <c>text/html</c>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The URL or URI at which the stored document can be located.</returns>
    Task<string> UploadAsync(
        string blobName,
        byte[] content,
        string contentType,
        CancellationToken cancellationToken = default);
}
