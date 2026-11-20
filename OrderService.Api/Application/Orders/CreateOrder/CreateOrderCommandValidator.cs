using FluentValidation;
using OrderService.Api.Application.Common;

namespace OrderService.Api.Application.Orders.CreateOrder
{
    public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId).RequiredGuid("El identificador de cliente");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("La lista de productos es obligatoria.")
                .NotEmpty().WithMessage("Debe enviar al menos un producto.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).RequiredGuid("El producto");
                item.RuleFor(i => i.Quantity).PositiveInt("La cantidad");
                item.RuleFor(i => i.UnitPrice).PositiveDecimal("El precio unitario");
            });
        }
    }
}
