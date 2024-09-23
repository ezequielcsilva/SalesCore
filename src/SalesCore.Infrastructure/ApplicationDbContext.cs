using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesCore.Application.Abstractions.Clock;
using SalesCore.Application.Abstractions.Data;

namespace SalesCore.Infrastructure;

internal sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IPublisher publisher,
    IDateTimeProvider dateTimeProvider) : DbContext, IDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}