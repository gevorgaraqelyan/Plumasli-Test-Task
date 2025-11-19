using Microsoft.AspNetCore.Mvc;
using backend.Common;

namespace backend.Common;

public static class ControllerExtensions
{
    public static IActionResult BadRequestError(this ControllerBase controller, string message)
    {
        return controller.BadRequest(new ApiErrorResponse(message));
    }

    public static IActionResult NotFoundError(this ControllerBase controller, string message)
    {
        return controller.NotFound(new ApiErrorResponse(message));
    }
}

