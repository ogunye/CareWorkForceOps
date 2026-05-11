namespace CareWorkOps.Application.Common
{
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("Successful result cannot contain an error.");
            }

            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException("Failed result must contain an error.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success()
        {
            return new Result(true, Error.None);
        }

        public static Result Failure(Error error)
        {
            return new Result(false, error);
        }
    }

    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        private Result(T value)
            : base(true, Error.None)
        {
            _value = value;
        }

        private Result(Error error)
            : base(false, error)
        {
            _value = default;
        }

        public T Value =>
            IsSuccess
                ? _value!
                : throw new InvalidOperationException("Cannot access value of a failed result.");

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        public static new Result<T> Failure(Error error)
        {
            return new Result<T>(error);
        }
    }
}
