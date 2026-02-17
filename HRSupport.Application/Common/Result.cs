using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HRSupport.Application.Common
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public Result() { }
        public Result(T data, string Message)
        {   
            Succeeded = true;
            Message = Message;
            Data = data;
        }
      public Result (string message)
            {
            Succeeded = true;
            Message = message;
        }
        public static Result<T> Success(T data, string message = "iþlem baþarýlý")
        {
            return new Result<T>(data, Message);
        }
        public static Result<T> Failure(List<string> errors, string message = "iþlem baþarýsýz")
        {
            return new Result<T>(Errors, Message);
        }
    }
}
