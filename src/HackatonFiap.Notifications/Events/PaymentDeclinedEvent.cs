namespace HackatonFiap.Notifications.Events;

/// <summary>
/// Resultado de pagamento recusado publicado pela PaymentAPI no tópico "payment-result"
/// com Subject "PaymentDeclined". Espelha o contrato de
/// <c>HackatonFiap.Payments.Application.IntegrationEvents.PaymentDeclinedEvent</c>.
/// </summary>
public sealed record PaymentDeclinedEvent(
    Guid DonationId,
    Guid CampaignId,
    string Reason,
    decimal Amount,
    Guid DonorId,
    string DonorEmail,
    string DonorName);
