using System.Net;

namespace Application.DTOs.Responses;

public class ApiResponse<T>
{
    public T? Content { get; }
    public string? ErrorMessage { get; }
    public HttpStatusCode? ErrorCode { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public ApiResponse(HttpStatusCode errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
    
    public ApiResponse(T content)
    {
        Content = content;
    }
}