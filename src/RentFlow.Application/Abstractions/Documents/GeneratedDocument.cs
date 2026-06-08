namespace RentFlow.Application.Abstractions.Documents;

public sealed record GeneratedDocument(byte[] Content, string ContentType, string FileExtension);
