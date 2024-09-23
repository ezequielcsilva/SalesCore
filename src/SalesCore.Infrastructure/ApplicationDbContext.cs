using Microsoft.EntityFrameworkCore;
using SalesCore.Application.Abstractions.Data;

namespace SalesCore.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options), IDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}