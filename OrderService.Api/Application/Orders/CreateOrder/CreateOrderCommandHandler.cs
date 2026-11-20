using MediatR;
using OrderService.Api.Domain.Common;
using OrderService.Api.Domain.Orders;

namespace OrderService.Api.Application.Orders.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var domainItems = request.Items
                .Select(i => new OrderItemInput(i.ProductId, i.Quantity, i.UnitPrice));

            var orderResult = Order.Create(request.CustomerId, domainItems);
            if (orderResult.IsFailure)
            {
                return Task.FromResult(Result<Guid>.Failure(orderResult.Errors));
            }

            // Aquí iría persistencia en repositorio / UnitOfWork
            return Task.FromResult(Result<Guid>.Success(orderResult.Value!.Id));
        }
    }
}
