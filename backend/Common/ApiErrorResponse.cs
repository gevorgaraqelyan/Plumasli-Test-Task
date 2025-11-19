namespace backend.Common;

public class ApiErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }

    public ApiErrorResponse(string error, string? details = null)
    {
        Error = error;
        Details = details;
    }
}

