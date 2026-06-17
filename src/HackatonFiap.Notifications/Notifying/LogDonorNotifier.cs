using HackatonFiap.Notifications.Events;
using Microsoft.Extensions.Logging;

namespace HackatonFiap.Notifications.Notifying;

/// <summary>
/// Canal de notificação mock/log (RN07.2): registra a notificação ao doador em log
/// estruturado (Console + Application Insights). Não há provedor externo de email/SMS no MVP,
/// coerente com o gateway de pagamento simulado.
/// </summary>
public sealed class LogDonorNotifier(ILogger<LogDonorNotifier> logger) : IDonorNotifier
{
    public Task NotifyApprovedAsync(PaymentApprovedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[Notificação] Doação aprovada — notificando doador {DonorName} <{DonorEmail}>. " +
            "DoacaoId={DonationId}, CampanhaId={CampaignId}, Valor={Amount}, PagamentoId={PaymentId}",
            paymentEvent.DonorName, paymentEvent.DonorEmail, paymentEvent.DonationId,
            paymentEvent.CampaignId, paymentEvent.Amount, paymentEvent.PaymentId);

        return Task.CompletedTask;
    }

    public Task NotifyDeclinedAsync(PaymentDeclinedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[Notificação] Pagamento recusado — notificando doador {DonorName} <{DonorEmail}>. " +
            "DoacaoId={DonationId}, CampanhaId={CampaignId}, Valor={Amount}, Motivo={Reason}",
            paymentEvent.DonorName, paymentEvent.DonorEmail, paymentEvent.DonationId,
            paymentEvent.CampaignId, paymentEvent.Amount, paymentEvent.Reason);

        return Task.CompletedTask;
    }
}
