using OrderService.Api.Application.Orders.CreateOrder;
using FluentValidation.TestHelper;
using Xunit;

namespace OrderServiceTest
{
    public sealed class CreateOrderCommandValidatorTests
    {
        private readonly CreateOrderCommandValidator _validator = new();

        [Fact]
        public void Should_Fail_When_CustomerId_Is_Empty()
        {
            var command = new CreateOrderCommand(
                CustomerId: Guid.Empty,
                Items: [new CreateOrderItemRequest(Guid.NewGuid(), Quantity: 1, UnitPrice: 10m)]);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Fact]
        public void Should_Fail_When_Items_List_Is_Empty()
        {
            var command = new CreateOrderCommand(
                CustomerId: Guid.NewGuid(),
                Items: []);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Items);
        }

        [Fact]
        public void Should_Fail_When_Item_Quantity_Is_Zero_Or_Negative()
        {
            var command = new CreateOrderCommand(
                CustomerId: Guid.NewGuid(),
                Items: [new CreateOrderItemRequest(Guid.NewGuid(), Quantity: 0, UnitPrice: 25m)]);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
        }

        [Fact]
        public void Should_Pass_When_Command_Is_Valid()
        {
            var command = new CreateOrderCommand(
                CustomerId: Guid.NewGuid(),
                Items:
                [
                    new CreateOrderItemRequest(Guid.NewGuid(), Quantity: 2, UnitPrice: 15.50m),
                new CreateOrderItemRequest(Guid.NewGuid(), Quantity: 1, UnitPrice: 10.00m)
                ]);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
