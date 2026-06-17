namespace HackatonFiap.Notifications.Events;

/// <summary>
/// Resultado de pagamento aprovado publicado pela PaymentAPI no tópico "payment-result"
/// com Subject "PaymentApproved". Espelha o contrato de
/// <c>HackatonFiap.Payments.Application.IntegrationEvents.PaymentApprovedEvent</c>.
/// </summary>
public sealed record PaymentApprovedEvent(
    Guid DonationId,
    Guid CampaignId,
    decimal Amount,
    Guid PaymentId,
    Guid DonorId,
    string DonorEmail,
    string DonorName);
