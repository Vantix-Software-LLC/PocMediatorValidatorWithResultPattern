using FluentValidation;

namespace OrderService.Api.Application.Common
{
    public static class ValidationRules
    {
        public static IRuleBuilderOptions<T, Guid> RequiredGuid<T>(
            this IRuleBuilder<T, Guid> rule,
            string fieldLabel) =>
            rule.NotEmpty().WithMessage($"{fieldLabel} es obligatorio.");

        public static IRuleBuilderOptions<T, int> PositiveInt<T>(
            this IRuleBuilder<T, int> rule,
            string fieldLabel) =>
            rule.GreaterThan(0).WithMessage($"{fieldLabel} debe ser mayor que cero.");

        public static IRuleBuilderOptions<T, decimal> PositiveDecimal<T>(
            this IRuleBuilder<T, decimal> rule,
            string fieldLabel) =>
            rule.GreaterThan(0m).WithMessage($"{fieldLabel} debe ser mayor que cero.");
    }
}
