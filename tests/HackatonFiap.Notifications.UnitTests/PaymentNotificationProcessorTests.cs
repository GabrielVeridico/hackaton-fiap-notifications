using System.Text.Json;
using FluentAssertions;
using HackatonFiap.Notifications.Events;
using HackatonFiap.Notifications.Notifying;
using HackatonFiap.Notifications.Processing;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HackatonFiap.Notifications.UnitTests;

public class PaymentNotificationProcessorTests
{
    private readonly IDonorNotifier _notifier = Substitute.For<IDonorNotifier>();
    private readonly PaymentNotificationProcessor _sut;

    public PaymentNotificationProcessorTests()
    {
        _sut = new PaymentNotificationProcessor(
            _notifier, Substitute.For<ILogger<PaymentNotificationProcessor>>());
    }

    [Fact]
    public async Task ProcessAsync_WithApprovedSubject_NotifiesDonorOfApproval()
    {
        var paymentEvent = new PaymentApprovedEvent(
            Guid.NewGuid(), Guid.NewGuid(), 100.50m, Guid.NewGuid(),
            Guid.NewGuid(), "doador@test.com", "Maria");
        var body = JsonSerializer.Serialize(paymentEvent);

        await _sut.ProcessAsync(PaymentNotificationProcessor.ApprovedSubject, body);

        await _notifier.Received(1).NotifyApprovedAsync(
            Arg.Is<PaymentApprovedEvent>(e =>
                e.DonationId == paymentEvent.DonationId &&
                e.DonorEmail == "doador@test.com" &&
                e.Amount == 100.50m),
            Arg.Any<CancellationToken>());
        await _notifier.DidNotReceive().NotifyDeclinedAsync(
            Arg.Any<PaymentDeclinedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_WithDeclinedSubject_NotifiesDonorOfDecline()
    {
        var paymentEvent = new PaymentDeclinedEvent(
            Guid.NewGuid(), Guid.NewGuid(), "Saldo insuficiente", 99.99m,
            Guid.NewGuid(), "doador@test.com", "João");
        var body = JsonSerializer.Serialize(paymentEvent);

        await _sut.ProcessAsync(PaymentNotificationProcessor.DeclinedSubject, body);

        await _notifier.Received(1).NotifyDeclinedAsync(
            Arg.Is<PaymentDeclinedEvent>(e =>
                e.Reason == "Saldo insuficiente" &&
                e.DonorName == "João"),
            Arg.Any<CancellationToken>());
        await _notifier.DidNotReceive().NotifyApprovedAsync(
            Arg.Any<PaymentApprovedEvent>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("UnknownSubject")]
    [InlineData("")]
    [InlineData(null)]
    public async Task ProcessAsync_WithUnknownOrMissingSubject_DoesNotNotify(string? subject)
    {
        await _sut.ProcessAsync(subject, "{}");

        await _notifier.DidNotReceive().NotifyApprovedAsync(
            Arg.Any<PaymentApprovedEvent>(), Arg.Any<CancellationToken>());
        await _notifier.DidNotReceive().NotifyDeclinedAsync(
            Arg.Any<PaymentDeclinedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidJsonBody_DoesNotNotify()
    {
        await _sut.ProcessAsync(PaymentNotificationProcessor.ApprovedSubject, "not-a-json");

        await _notifier.DidNotReceive().NotifyApprovedAsync(
            Arg.Any<PaymentApprovedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyBody_DoesNotNotify()
    {
        await _sut.ProcessAsync(PaymentNotificationProcessor.DeclinedSubject, "");

        await _notifier.DidNotReceive().NotifyDeclinedAsync(
            Arg.Any<PaymentDeclinedEvent>(), Arg.Any<CancellationToken>());
    }
}
