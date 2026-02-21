namespace App.Core.DTOs.Common
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public static ApiResponseDto<T> Success(T data, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDto<T> Failure(string message, IEnumerable<string>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
