using MediatR;
using OrderService.Api.Domain.Common;

namespace OrderService.Api.Application.Orders.CancelOrder
{
    public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Result>;
}
