namespace StudentClub.Shared.ApiResponse
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public int Status { get; init; }
        public T? Data { get; init; }
        public string Message { get; init; } = string.Empty;
        public Dictionary<string, string[]> Errors { get; init; } = new();

        public static ApiResponse<T> Success(T data, string message = "") =>
            new() { Status = 200, IsSuccess = true, Data = data, Message = message };

        public static ApiResponse<T> Failure(int? status, string message, Dictionary<string, string[]>? errors = null) =>
            new() { Status = status ?? 500, IsSuccess = false, Message = message, Errors = errors ?? new() };
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; init; }
        public int Status { get; init; }
        public string Message { get; init; } = string.Empty;
        public Dictionary<string, string[]> Errors { get; init; } = new();

        public static ApiResponse Success(string message = "") =>
            new() { Status = 200, IsSuccess = true, Message = message };

        public static ApiResponse Failure(int? status, string message, Dictionary<string, string[]>? errors = null) =>
            new() { Status = status ?? 500, IsSuccess = false, Message = message, Errors = errors ?? new() };
    }

    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public long TotalCount { get; set; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public object GetMetadata()
        {
            return new
            {
                PageNumber,
                PageSize,
                TotalPages,
                TotalCount,
                HasPreviousPage,
                HasNextPage
            };
        }
    }
}
