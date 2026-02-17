namespace HRSupport.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T Value { get; private set; }
        public string Error { get; private set; }
        public List<string> Errors { get; private set; }
        private Result(bool isSuccess, T value, string error, List<string> errors)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Errors = errors;
        }
        public static Result<T> Success(T value, string message = null)
        {
            return new Result<T>(true, value, message, null);
        }
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default(T), error, null);
        }
        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, default(T), null, errors);
        }
    }
}