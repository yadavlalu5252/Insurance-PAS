using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Common
{
    public class ResponseWrapper<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T? Data { get; set; }

        public ResponseWrapper(
            bool success,
            string message,
            T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}

