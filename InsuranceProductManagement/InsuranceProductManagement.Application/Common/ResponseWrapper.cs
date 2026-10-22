
namespace InsuranceProductManagement.Application.Common
{
    public class ResponseWrapper<T>
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public T? data { get; set; }
    }

    public class SuccessResponse
    {
        public static ResponseWrapper<T> Create<T>(string message, T data)
        {
            return new ResponseWrapper<T>
            {
                success = true,
                message = message,
                data = data
            };
        }
    }

    public class ErrorResponse
    {
        public static ResponseWrapper<T> Create<T>(string message)
        {
            return new ResponseWrapper<T>
            {
                success = false,
                message = message,
                data = default
            };
        }
    }
}
