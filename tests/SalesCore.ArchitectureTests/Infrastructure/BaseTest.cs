using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;
using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Domain.Abstractions;
using SalesCore.Infrastructure;

namespace SalesCore.ArchitectureTests.Infrastructure;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    protected static readonly Assembly PresentationApiAssembly = typeof(Program).Assembly;
}