namespace HRSupport.UI.Models
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public string? Error { get; set; }
        public List<string>? Errors { get; set; }
    }
}