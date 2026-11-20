using OrderService.Api.Domain.Common;
using MediatR;

namespace OrderService.Api.Application.Orders.CreateOrder
{
    public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);

    public sealed record CreateOrderCommand(
        Guid CustomerId,
        IReadOnlyList<CreateOrderItemRequest> Items) : IRequest<Result<Guid>>;
}
