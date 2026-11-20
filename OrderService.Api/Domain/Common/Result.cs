namespace OrderService.Api.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors { get; }
        public Error FirstError => Errors.Count > 0 ? Errors[0] : Error.None;

        protected Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            if (isSuccess && errors.Count > 0)
                throw new InvalidOperationException("Un Result exitoso no puede tener errores.");

            if (!isSuccess && errors.Count == 0)
                throw new InvalidOperationException("Un Result fallido debe incluir al menos un error.");

            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success() => new(true, []);
        public static Result Failure(Error error) => new(false, [error]);
        public static Result Failure(IReadOnlyList<Error> errors) => new(false, errors);
    }

    public sealed class Result<TValue> : Result
    {
        public TValue? Value { get; }

        private Result(bool isSuccess, TValue? value, IReadOnlyList<Error> errors)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value) => new(true, value, []);
        public new static Result<TValue> Failure(Error error) => new(false, default, [error]);
        public new static Result<TValue> Failure(IReadOnlyList<Error> errors) => new(false, default, errors);
    }
}
