using OrderService.Api.Domain.Orders;

namespace OrderService.Api.Application.Orders.CancelOrder
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
