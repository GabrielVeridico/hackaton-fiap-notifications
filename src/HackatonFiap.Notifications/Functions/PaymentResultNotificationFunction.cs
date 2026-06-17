using Azure.Messaging.ServiceBus;
using HackatonFiap.Notifications.Processing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HackatonFiap.Notifications.Functions;

/// <summary>
/// Consome os eventos de resultado de pagamento da PaymentAPI no tópico "payment-result"
/// (subscription "notifications", independente da subscription "donations" da DonationAPI)
/// e dispara a notificação ao doador. Trigger Service Bus do Azure Functions isolated worker.
/// </summary>
public class PaymentResultNotificationFunction(
    PaymentNotificationProcessor processor,
    ILogger<PaymentResultNotificationFunction> logger)
{
    [Function(nameof(PaymentResultNotificationFunction))]
    public async Task Run(
        [ServiceBusTrigger("payment-result", "notifications", Connection = "SERVICEBUS_CONNECTION")]
        ServiceBusReceivedMessage message,
        CancellationToken cancellationToken)
    {
        var correlationId = !string.IsNullOrEmpty(message.CorrelationId)
            ? message.CorrelationId
            : message.MessageId;

        using var logScope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId ?? string.Empty,
            ["Subject"] = message.Subject ?? string.Empty
        });

        await processor.ProcessAsync(message.Subject, message.Body.ToString(), cancellationToken);
    }
}
