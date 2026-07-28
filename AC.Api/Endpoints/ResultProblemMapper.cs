using AC.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace AC.Api.Endpoints;

/// <summary>
/// Mapea un Result fallido a ProblemDetails con el status HTTP según el sufijo
/// del ErrorKey: .notfound → 404, .forbidden → 403, resto → 400.
/// </summary>
public static class ResultProblemMapper
{
    public static ActionResult ToProblem(this ControllerBase controller, Result result)
    {
        var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };

        if (result.ErrorKey.EndsWith(".notfound", StringComparison.Ordinal))
            return controller.NotFound(problem);

        if (result.ErrorKey.EndsWith(".forbidden", StringComparison.Ordinal))
            return controller.StatusCode(StatusCodes.Status403Forbidden, problem);

        return controller.BadRequest(problem);
    }
}
