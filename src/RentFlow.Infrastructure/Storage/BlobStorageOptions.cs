namespace RentFlow.Infrastructure.Storage;

/// <summary>Options for the document blob store, bound from the <c>BlobStorage</c> configuration section.</summary>
public sealed class BlobStorageOptions
{
    /// <summary>The configuration section name these options bind to.</summary>
    public const string SectionName = "BlobStorage";

    /// <summary>The blob container (Azure) or sub-folder (local) that documents are stored under.</summary>
    public string ContainerName { get; set; } = "contracts";

    /// <summary>
    /// The base directory used by the local file-system fallback. When unset, a
    /// <c>document-storage</c> folder under the application base directory is used.
    /// </summary>
    public string? LocalPath { get; set; }
}
