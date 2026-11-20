using FluentValidation;
using OrderService.Api.Application.Common;

namespace OrderService.Api.Application.Orders.CancelOrder
{
    public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).RequiredGuid("El identificador del pedido");
        }
    }
}
