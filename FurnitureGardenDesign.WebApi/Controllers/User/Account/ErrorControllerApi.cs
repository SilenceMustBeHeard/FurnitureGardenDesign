using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Account;

[Route("api/[controller]")]
[ApiController]
public class ErrorControllerApi : ControllerBase
{
    [HttpGet("{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        return statusCode switch
        {
            400 => BadRequest
           (new
           {
               error = "Bad Request",
               statusCode = 400,
               Message = "The request could not be understood by the server due to Invalid syntax."
           }),
            401 => Unauthorized
            (new
            {
                error = "Unauthorized",
                statusCode = 401,
                Message = "The request requires user authentication."
            }),
            403 => StatusCode
            (403, new
            {
                error = "Forbidden",
                statusCode = 403,
                Message = "The request is not allowed for the authenticated user."
            }),
            404 => NotFound
            (new
            {
                error = "Not Found",
                statusCode = 404,
                Message = "The requested resource was not found."
            }),
            501 => StatusCode
            (501, new
            {
                error = "Not Implemented",
                statusCode = 501,
                Message = "The requested feature is not implemented."
            }),
            _ => StatusCode
            (500, new
            {
                error = "Internal Server Error",
                statusCode = 500,
                Message = "An internal server error occurred."
            })
        };
    }

    [ApiExplorerSettings(IgnoreApi = true)] // Hide from API documentation
    public static IActionResult ReturnError(int statusCode, string message)
    {
        return new ObjectResult(new
        {
            error = GetDefaultErrorTitle(statusCode),
            statusCode,
            message
        })
        {
            StatusCode = statusCode
        };
    }

    private static string GetDefaultErrorTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            422 => "Unprocessable Entity",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            _ => "Error"
        };
    }
}