using HackatonFiap.Notifications.Events;

namespace HackatonFiap.Notifications.Notifying;

/// <summary>
/// Canal de notificação ao doador. No MVP a implementação é mock/log (RN07.2);
/// o contrato permite trocar por um provedor real (email/SMS) sem tocar na orquestração.
/// </summary>
public interface IDonorNotifier
{
    Task NotifyApprovedAsync(PaymentApprovedEvent paymentEvent, CancellationToken cancellationToken = default);

    Task NotifyDeclinedAsync(PaymentDeclinedEvent paymentEvent, CancellationToken cancellationToken = default);
}
