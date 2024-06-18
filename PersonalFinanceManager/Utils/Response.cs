using Microsoft.AspNetCore.Mvc;
namespace PersonalFinanceManager.Utils
{
    static public class Response
    {

        public const string DEFAULT_FORBIDDEN_DETAIL = "This resource doesn't belong to you!!!";
        public const string DEFAULT_NOTFOUND_DETAIL  = "This resource doesn't exist";
        public const string DEFAULT_UNAUTHORIZED_DETAIL = "Wrong or non existing credentials!";
        public const string DEFAULT_BADREQUEST_DETAIL = "One or more validation errors occurred.";


        public static ActionResult Forbidden(ControllerBase controller,string detail = DEFAULT_FORBIDDEN_DETAIL) 
        {
            return controller.StatusCode(StatusCodes.Status403Forbidden,new ProblemDetails {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = detail,
                Instance = controller.HttpContext.Request.Path
            });
        }

        public static ActionResult NotFound(ControllerBase controller, string detail = DEFAULT_NOTFOUND_DETAIL)
        {
            return controller.StatusCode(StatusCodes.Status404NotFound, new ProblemDetails
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = detail,
                Instance = controller.HttpContext.Request.Path
            });
        }


        public static ActionResult Unauthorized(ControllerBase controller, string detail = DEFAULT_UNAUTHORIZED_DETAIL)
        {
            return controller.StatusCode(StatusCodes.Status401Unauthorized, new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = detail,
                Instance = controller.HttpContext.Request.Path,
                Extensions = new Dictionary<string, object?> { {"Basic Auth reference","https://datatracker.ietf.org/doc/html/rfc7617" } }
            });
        }
    }
}
