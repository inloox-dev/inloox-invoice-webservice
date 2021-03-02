using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InLooxInvoiceWebservice.Helpers
{
    public static class ActionResultHelper
    {
        public static ActionResult GeneralResult(string messageString, int statusCodeInt)
        {
            return new ObjectResult(new { statusCode = statusCodeInt, message = messageString })
            {
                StatusCode = statusCodeInt
            };
        }

        public static ActionResult UnauthorizedResult(string messageString)
        {
            var statusCodeInt = StatusCodes.Status401Unauthorized;
            return GeneralResult(messageString, statusCodeInt);
        }

        public static ActionResult ServerErrorResult(string messageString)
        {
            var statusCodeInt = StatusCodes.Status500InternalServerError;
            return GeneralResult(messageString, statusCodeInt);
        }
    }
}
