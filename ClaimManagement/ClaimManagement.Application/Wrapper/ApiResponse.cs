namespace ClaimManagement.Application.Wrapper
{
    public class ApiResponse<T>
    {
        public bool success { get; set; }

        public string message { get; set; } = string.Empty;

        public T? data { get; set; }

        public List<string>? errors { get; set; }

        public static ApiResponse<T> Ok(T? data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                success = true,
                message = message,
                data = data
            };
        }

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                success = false,
                message = message,
                errors = errors
            };
        }
    }
}