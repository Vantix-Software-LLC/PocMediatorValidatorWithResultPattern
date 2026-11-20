using OrderService.Api.Application.Orders.CancelOrder;
using OrderService.Api.Domain.Common.Enums;
using OrderService.Api.Domain.Orders;

namespace OrderServiceTest
{
    public sealed class CancelOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_NotFound_When_Order_Does_Not_Exist()
        {
            var repository = new FakeOrderRepository(existingOrder: null);
            var handler = new CancelOrderCommandHandler(repository);

            var result = await handler.Handle(new CancelOrderCommand(Guid.NewGuid()), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
            Assert.False(repository.Saved);
        }

        private sealed class FakeOrderRepository(Order? existingOrder) : IOrderRepository
        {
            public bool Saved { get; private set; }

            public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken) =>
                Task.FromResult(existingOrder);

            public Task SaveChangesAsync(CancellationToken cancellationToken)
            {
                Saved = true;
                return Task.CompletedTask;
            }
        }
    }
}
