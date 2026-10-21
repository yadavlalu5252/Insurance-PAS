
namespace KycVerification.Api.Wrappers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string Error { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ApiResponse()
        {
        }

        public ApiResponse(bool success, string message, T? data, string error = "")
        {
            Success = success;
            Message = message;
            Data = data;
            Error = error;
            Timestamp = DateTime.UtcNow;
        }
    }
}
