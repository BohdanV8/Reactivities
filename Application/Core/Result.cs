using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Core
{
    public class Result<T>
    {
        public bool isSuccess { get; set; }
        public T? Value { get; set; }
        public string? Error { get; set; }
        public int Code { get; set; }

        public static Result<T> Success(T value)
        {
            return new Result<T> { isSuccess = true, Value = value }; 
        }

        public static Result<T> Failure(string error, int code)
        {
            return new Result<T> { isSuccess = false, Error = error, Code = code };
        }
    }
}
