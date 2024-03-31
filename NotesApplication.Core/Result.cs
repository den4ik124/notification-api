using System.Net;

namespace NotesApplication.Core;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public static Result Success() => new Result()
    {
        IsSuccess = true,
        StatusCode = HttpStatusCode.OK
    };

    public static Result Fail(string message, HttpStatusCode statusCode) => new Result() { Message = message, StatusCode = statusCode };
}