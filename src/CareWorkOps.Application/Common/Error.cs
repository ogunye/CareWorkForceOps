namespace CareWorkOps.Application.Common
{
    public sealed record Error(string Code, string Message)
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public static Error Validation(string message)
        {
            return new Error("Validation.Error", message);
        }

        public static Error Conflict(string message)
        {
            return new Error("Conflict.Error", message);
        }

        public static Error Failure(string message)
        {
            return new Error("Failure.Error", message);
        }
    }
}
