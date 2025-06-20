using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Wolt.MiddleWares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails;

            switch (exception)
            {
                case ArgumentNullException argNullEx:
                    problemDetails = new ProblemDetails()
                    {
                        Title = "BadRequest",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = argNullEx.Message
                    };
                    break;

                case NullReferenceException nullRefEx:
                    problemDetails = new ProblemDetails()
                    {
                        Title = "Internal Server Error",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = "An internal error occurred. Please make sure you're logged in and try again."
                    };
                    break;

                case UnauthorizedAccessException unauthEx:
                    problemDetails = new ProblemDetails()
                    {
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = unauthEx.Message
                    };
                    break;

                case NotImplementedException notImpEx:
                    problemDetails = new ProblemDetails()
                    {
                        Title = "Not Implemented",
                        Status = StatusCodes.Status501NotImplemented,
                        Detail = notImpEx.Message
                    };
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    problemDetails = new ProblemDetails()
                    {
                        Title = "Not Found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = keyNotFoundEx.Message
                    };
                    break;

                default:
                    problemDetails = new ProblemDetails()
                    {
                        Title = "An unexpected error occured",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = exception.Message
                    };
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
