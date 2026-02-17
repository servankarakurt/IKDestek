namespace HRSupport.Application.Common
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public bool IsSuccess => Succeeded;
        public T Value=>Data;
        public List<string> Error => Errors;
        public Result()
        {
            Errors = new List<string>();
        }
        public Result(T data, string Message)        {
            Succeeded = true;
            Message = message;
            Data = data;
            Errors = null;
        }
        public Result( List<string> errors,string message)
        {
            Succeeded = true;
            Message = message;
            Data= default;
            Errors = errors;
        }
        public static Result<T> Success(T data, string message = "iþlem baþarýlý")
        {
            return new Result<T> { Succeeded = true, Data = data, Message = message };
        }
        public static Result<T> Failure(List<string> errors, string message = "iþlem baþarýsýz")
        {
            return new Result<T>(errors, message);
        }
    }
}
