using OrderService.Api.Domain.Common.Enums;
using OrderService.Api.Domain.Orders;
using Xunit;

namespace OrderServiceTest
{
   

    public sealed class OrderTests
    {
        [Fact]
        public void Create_Should_Calculate_Total_And_Return_Success_When_Items_Are_Valid()
        {
            var customerId = Guid.NewGuid();
            var items = new[]
            {
            new OrderItemInput(Guid.NewGuid(), Quantity: 2, UnitPrice: 10.00m),
            new OrderItemInput(Guid.NewGuid(), Quantity: 3, UnitPrice: 5.00m)
        };

            var result = Order.Create(customerId, items);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(customerId, result.Value!.CustomerId);
            Assert.Equal(35.00m, result.Value.Total);
            Assert.Equal(2, result.Value.Items.Count);
        }

        [Fact]
        public void Create_Should_Return_Conflict_When_Items_Are_Empty()
        {
            var result = Order.Create(Guid.NewGuid(), []);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Conflict, result.FirstError.Type);
            Assert.Equal("Orders.EmptyItems", result.FirstError.Code);
        }

        [Fact]
        public void Create_Should_Return_Conflict_When_UnitPrice_Is_Zero()
        {
            var items = new[]
            {
            new OrderItemInput(Guid.NewGuid(), Quantity: 1, UnitPrice: 0m)
        };

            var result = Order.Create(Guid.NewGuid(), items);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Conflict, result.FirstError.Type);
            Assert.Equal("Orders.InvalidItemValues", result.FirstError.Code);
        }
    }
}
