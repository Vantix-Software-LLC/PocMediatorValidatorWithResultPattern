using OrderService.Api.Domain.Common;
using OrderService.Api.Domain.Common.Enums;

namespace OrderService.Api.Domain.Orders
{
    public sealed record OrderItemInput(Guid ProductId, int Quantity, decimal UnitPrice);
    public class   Order
 {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Result<Order> Create(Guid customerId, IEnumerable<OrderItemInput> items)
    {
        var itemList = items.ToList();
        if (itemList.Count == 0)
        {
            return Result<Order>.Failure(Error.Conflict(
                "Orders.EmptyItems",
                "El pedido debe contener al menos un producto."));
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId
        };

        foreach (var item in itemList)
        {
            if (item.Quantity <= 0 || item.UnitPrice <= 0)
            {
                return Result<Order>.Failure(Error.Conflict(
                    "Orders.InvalidItemValues",
                    "Cantidad y precio unitario deben ser mayores que cero."));
            }

            order._items.Add(new OrderItem(item.ProductId, item.Quantity, item.UnitPrice));
        }

        order.Total = order._items.Sum(i => i.Quantity * i.UnitPrice);
        return Result<Order>.Success(order);
    }
        public Result Cancel()
        {
            if (Status == OrderStatus.Cancelled)
            {
                return Result.Failure(Error.Conflict(
                    "Orders.AlreadyCancelled",
                    "El pedido ya se encuentra cancelado."));
            }

            if (Status == OrderStatus.Shipped)
            {
                return Result.Failure(Error.Conflict(
                    "Orders.AlreadyShipped",
                    "No se puede cancelar un pedido que ya fue enviado."));
            }

            Status = OrderStatus.Cancelled;
            return Result.Success();
        }
    }

public sealed record OrderItem(Guid ProductId, int Quantity, decimal UnitPrice);
}
