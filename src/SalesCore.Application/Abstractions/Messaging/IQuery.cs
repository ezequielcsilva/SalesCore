using MediatR;
using SalesCore.Domain.Abstractions;

namespace SalesCore.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery;

public interface IBaseQuery;