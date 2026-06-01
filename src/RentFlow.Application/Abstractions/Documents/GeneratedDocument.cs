namespace RentFlow.Application.Abstractions.Documents;

/// <summary>A rendered document ready to be persisted to the document store.</summary>
/// <param name="Content">The rendered document bytes.</param>
/// <param name="ContentType">The MIME content type, for example <c>text/html</c>.</param>
/// <param name="FileExtension">The file extension including the leading dot, for example <c>.html</c>.</param>
public sealed record GeneratedDocument(byte[] Content, string ContentType, string FileExtension);
