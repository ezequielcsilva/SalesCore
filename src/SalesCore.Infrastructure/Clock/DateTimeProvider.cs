using SalesCore.Application.Abstractions.Clock;

namespace SalesCore.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}