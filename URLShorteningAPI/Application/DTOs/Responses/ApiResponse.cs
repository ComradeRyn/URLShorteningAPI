using System.Net;

namespace Application.DTOs.Responses;

public class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; }
    public T? Content { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public ApiResponse(HttpStatusCode statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public ApiResponse(HttpStatusCode statusCode, T content)
    {
        StatusCode = statusCode;
        Content = content;
    }
}