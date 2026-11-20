using MediatR;
using OrderService.Api.Domain.Common;

namespace OrderService.Api.Application.Orders.CancelOrder
{
    public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository)
       : IRequestHandler<CancelOrderCommand, Result>
    {
        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                return Result.Failure(Error.NotFound(
                    "Orders.NotFound",
                    $"No se encontró el pedido con ID {request.OrderId}."));
            }

            var cancelResult = order.Cancel();
            if (cancelResult.IsFailure)
            {
                return cancelResult;
            }

            await orderRepository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
