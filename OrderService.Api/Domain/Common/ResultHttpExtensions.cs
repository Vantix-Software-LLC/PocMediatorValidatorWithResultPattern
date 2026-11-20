using OrderService.Api.Domain.Common.Enums;

namespace OrderService.Api.Domain.Common
{
    public static class ResultHttpExtensions
    {
        public static IResult ToHttpProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("No se puede mapear un Result exitoso a error HTTP.");

            if (result.FirstError is ValidationError validationError)
            {
                return Results.ValidationProblem(
                    errors: validationError.FieldErrors.ToDictionary(k => k.Key, v => v.Value),
                    title: validationError.Message,
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?> { ["code"] = validationError.Code });
            }

            var error = result.FirstError;
            var (statusCode, title) = error.Type switch
            {
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflicto de negocio"),
                _ => (StatusCodes.Status422UnprocessableEntity, "Error al procesar la solicitud")
            };

            return Results.Problem(
                statusCode: statusCode,
                title: title,
                detail: error.Message,
                extensions: new Dictionary<string, object?> { ["code"] = error.Code });
        }
    }
}
