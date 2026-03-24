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

    // TODO: does not make much sense to always have it be OK, because there exists other accepting status codes
    public ApiResponse(T content)
    {
        StatusCode = HttpStatusCode.OK;
        Content = content;
    }
}