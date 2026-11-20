using OrderService.Api.Domain.Common.Enums;

namespace OrderService.Api.Domain.Common
{
    public sealed record ValidationError(
     string Code,
     string Message,
     IReadOnlyDictionary<string, string[]> FieldErrors)
     : Error(Code, Message, ErrorType.Validation);
}
