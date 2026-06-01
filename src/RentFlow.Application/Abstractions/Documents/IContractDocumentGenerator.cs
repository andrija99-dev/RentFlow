namespace RentFlow.Application.Abstractions.Documents;

/// <summary>Renders a printable rental-contract document from its data.</summary>
public interface IContractDocumentGenerator
{
    /// <summary>Renders the contract document.</summary>
    /// <param name="data">The contract data to render.</param>
    /// <returns>The rendered document together with its content type and extension.</returns>
    GeneratedDocument Generate(ContractDocumentData data);
}
