using System.Text.Json;
using System.Text.Json.Serialization;
using HackatonFiap.Notifications.Events;
using HackatonFiap.Notifications.Notifying;
using Microsoft.Extensions.Logging;

namespace HackatonFiap.Notifications.Processing;

/// <summary>
/// Orquestra a notificação a partir do resultado de pagamento publicado pela PaymentAPI
/// no tópico "payment-result". O tipo do evento é distinguido pelo <c>Subject</c> da mensagem
/// (PaymentApproved / PaymentDeclined), exatamente como o publisher da PaymentAPI o marca —
/// o corpo da mensagem não carrega um campo de status.
/// </summary>
public sealed class PaymentNotificationProcessor(
    IDonorNotifier notifier,
    ILogger<PaymentNotificationProcessor> logger)
{
    public const string ApprovedSubject = "PaymentApproved";
    public const string DeclinedSubject = "PaymentDeclined";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task ProcessAsync(string? subject, string body, CancellationToken cancellationToken = default)
    {
        switch (subject)
        {
            case ApprovedSubject:
                var approved = Deserialize<PaymentApprovedEvent>(body);
                if (approved is not null)
                {
                    await notifier.NotifyApprovedAsync(approved, cancellationToken);
                }
                break;

            case DeclinedSubject:
                var declined = Deserialize<PaymentDeclinedEvent>(body);
                if (declined is not null)
                {
                    await notifier.NotifyDeclinedAsync(declined, cancellationToken);
                }
                break;

            default:
                logger.LogWarning(
                    "[Notificação] Subject desconhecido ou ausente: '{Subject}'. Mensagem ignorada.", subject);
                break;
        }
    }

    private T? Deserialize<T>(string body) where T : class
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            logger.LogWarning("[Notificação] Corpo vazio para {EventType}. Mensagem ignorada.", typeof(T).Name);
            return null;
        }

        try
        {
            var paymentEvent = JsonSerializer.Deserialize<T>(body, JsonOptions);
            if (paymentEvent is null)
            {
                logger.LogWarning("[Notificação] Payload nulo para {EventType}. Mensagem ignorada.", typeof(T).Name);
            }
            return paymentEvent;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "[Notificação] Falha ao desserializar {EventType}. Mensagem ignorada.", typeof(T).Name);
            return null;
        }
    }
}
