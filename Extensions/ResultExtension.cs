using System;
using FriendStuff.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using FriendStuff.Shared.Results.Enums;

namespace FriendStuff.Extensions;

public static class ResultExtension
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(new { response = result.SuccessMessage });

        return MapError(result.Error);


    }
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(new { response = result.SuccessMessage, value = result.Value });

        return MapError(result.Error);

    }

    private static ObjectResult MapError(Error error)
    {
        var response = error.Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(error),
            ErrorType.Forbidden => new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden },
            ErrorType.Conflict => new ConflictObjectResult(error),
            ErrorType.Validation => new BadRequestObjectResult(error),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(error),
            _ => new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError }
        };
        return response;
    }
}
