using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Services;

namespace WCCG.eReferralsService.Unit.Tests.Services;

public class AuditLogServiceTests
{
    [Fact]
    public async Task LogAsyncShouldWriteStructuredAuditLogEntry()
    {
        // Arrange
        var logger = new ListLogger<AuditLogService>();
        var sut = new AuditLogService(logger);

        var headers = new HeaderDictionary
        {
            { RequestHeaderKeys.RequestId, "req-1" },
            { RequestHeaderKeys.CorrelationId, "corr-1" },
            { RequestHeaderKeys.EndUserOrganisation, "org-1" },
            { RequestHeaderKeys.RequestingSoftware, "soft-1" },
        };

        // Act
        await sut.LogAsync(headers, AuditEvents.HeadersValidationSucceeded);

        // Assert
        logger.Entries.Should().HaveCount(1);
        var entry = logger.Entries[0];

        entry.Level.Should().Be(LogLevel.Information);
        entry.Exception.Should().BeNull();
        entry.Message.Should().Contain("AuditLog");
        entry.Message.Should().Contain("AuditEvent=HeadersValidationSucceeded");
        entry.Message.Should().Contain("TimestampUtc=");
        entry.Message.Should().Contain("X-Request-Id=req-1");
        entry.Message.Should().Contain("X-Correlation-Id=corr-1");
        entry.Message.Should().Contain("EndUserOrganisation=org-1");
        entry.Message.Should().Contain("RequestingSoftware=soft-1");
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            IReadOnlyList<KeyValuePair<string, object?>>? stateValues = state as IReadOnlyList<KeyValuePair<string, object?>>
                ?? (state as IEnumerable<KeyValuePair<string, object?>>)?.ToList();

            Entries.Add(new LogEntry(
                logLevel,
                eventId,
                formatter(state, exception),
                exception,
                stateValues));
        }

        public sealed record LogEntry(
            LogLevel Level,
            EventId EventId,
            string Message,
            Exception? Exception,
            IReadOnlyList<KeyValuePair<string, object?>>? State);

        private sealed class NullDisposable : IDisposable
        {
            public static NullDisposable Instance { get; } = new();
            public void Dispose() { }
        }
    }
}
