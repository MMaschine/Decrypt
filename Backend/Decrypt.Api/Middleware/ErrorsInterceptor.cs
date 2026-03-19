using System.Net;
using Decrypt.Api.Models;
using Decrypt.Api.Models.Exceptions;

namespace Decrypt.Api.Middleware
{
    public class ErrorsInterceptor
    {
        /// <summary>
        /// HTTP response content type.
        /// </summary>
        private const string ContentType = "application/json";

        //TODO: In case of implementing logging add logger and loggin in exception catch blocks

        private readonly RequestDelegate _next;

        public ErrorsInterceptor(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (ApiErrorException apiErrorEx)
            {
                await HandleError(context, apiErrorEx.ApiError);
            }
            catch (Exception ex)
            {
                var error = new ApiError
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };

                await HandleError(context, error);
            }
        }

        private Task HandleError(HttpContext context, ApiError error)
        {
            context.Response.ContentType = ContentType;
            context.Response.StatusCode = (int)error.StatusCode;

            var errorJson = error.ToString();
            return context.Response.WriteAsync(errorJson);
        }
    }
}
