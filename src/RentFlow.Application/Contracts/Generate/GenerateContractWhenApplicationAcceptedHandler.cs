using Microsoft.Extensions.Logging;
using RentFlow.Application.Abstractions.Documents;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Abstractions.Storage;
using RentFlow.Application.DomainEvents;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Events;
using RentFlow.Domain.Interfaces;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Application.Contracts.Generate;

/// <summary>
/// Observer that generates a rental contract when an application is accepted. It
/// applies the standard fixed lease term, renders and uploads the contract document
/// to the document store, and persists the contract. Idempotent: if a contract
/// already exists for the application it does nothing, so a re-published event never
/// produces a duplicate.
/// </summary>
internal sealed class GenerateContractWhenApplicationAcceptedHandler(
    IContractRepository contracts,
    IPropertyReadService propertyReads,
    IContractDocumentGenerator documentGenerator,
    IDocumentStorage documentStorage,
    IUnitOfWork unitOfWork,
    ILogger<GenerateContractWhenApplicationAcceptedHandler> logger)
    : IDomainEventHandler<RentalApplicationAcceptedEvent>
{
    /// <summary>The standard fixed lease term applied to a generated contract.</summary>
    private const int LeaseTermMonths = 12;

    private readonly IContractRepository _contracts = contracts;
    private readonly IPropertyReadService _propertyReads = propertyReads;
    private readonly IContractDocumentGenerator _documentGenerator = documentGenerator;
    private readonly IDocumentStorage _documentStorage = documentStorage;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GenerateContractWhenApplicationAcceptedHandler> _logger = logger;

    public async Task Handle(
        DomainEventNotification<RentalApplicationAcceptedEvent> notification,
        CancellationToken cancellationToken)
    {
        var accepted = notification.DomainEvent;

        if (await _contracts.GetByApplicationAsync(accepted.ApplicationId, cancellationToken) is not null)
        {
            return;
        }

        var property = await _propertyReads.GetByIdAsync(accepted.PropertyId, cancellationToken);
        if (property is null)
        {
            _logger.LogWarning(
                "Skipping contract generation for application {ApplicationId}: property {PropertyId} no longer exists.",
                accepted.ApplicationId,
                accepted.PropertyId);
            return;
        }

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        var endDate = startDate.AddMonths(LeaseTermMonths);
        var monthlyRent = Money.Create(property.Price.Amount, property.Price.Currency);

        var contract = Contract.Create(accepted.ApplicationId, startDate, endDate, monthlyRent);

        var document = _documentGenerator.Generate(new ContractDocumentData(
            contract.Id,
            property.Title,
            property.Address.Street,
            property.Address.City,
            property.Address.PostalCode,
            property.Address.Country,
            property.OwnerId,
            accepted.TenantId,
            startDate,
            endDate,
            monthlyRent.Amount,
            monthlyRent.Currency));

        var blobName = $"{contract.Id}{document.FileExtension}";
        var documentUrl = await _documentStorage
            .UploadAsync(blobName, document.Content, document.ContentType, cancellationToken)
            .ConfigureAwait(false);

        contract.AttachDocument(documentUrl);

        await _contracts.AddAsync(contract, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Generated contract {ContractId} for accepted application {ApplicationId}.",
            contract.Id,
            accepted.ApplicationId);
    }
}
