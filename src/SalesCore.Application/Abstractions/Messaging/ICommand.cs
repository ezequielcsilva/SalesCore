using MediatR;
using SalesCore.Domain.Abstractions;

namespace SalesCore.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}