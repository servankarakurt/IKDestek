namespace HRSupport.Application.Common
{
    public interface IResult
    {
        bool IsSuccess { get; }
    }

    public class Result<T> : IResult
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public string Error { get; private set; } = string.Empty;
        public List<string> Errors { get; private set; } = new List<string>();
        private Result(bool isSuccess, T? value, string? error, List<string>? errors)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error ?? string.Empty;
            Errors = errors ?? new List<string>();
        }
        public static Result<T> Success(T value, string? message = null)
        {
            return new Result<T>(true, value, message, null);
        }
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error ?? string.Empty, null);
        }
        public static Result<T> Failure(List<string>? errors)
        {
            return new Result<T>(false, default, null, errors);
        }
    }
}