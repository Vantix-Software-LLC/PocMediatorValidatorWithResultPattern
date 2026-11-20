using OrderService.Api.Application.Orders.CreateOrder;
using System;

namespace OrderServiceTest
{
    public sealed class CreateOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Success_With_OrderId_When_Command_Is_Valid()
        {
            var handler = new CreateOrderCommandHandler();
            var command = new CreateOrderCommand(
                CustomerId: Guid.NewGuid(),
                Items: [new CreateOrderItemRequest(Guid.NewGuid(), Quantity: 2, UnitPrice: 19.99m)]);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        }
    }
}
