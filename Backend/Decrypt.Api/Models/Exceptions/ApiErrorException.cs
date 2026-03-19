using System.Net;

namespace Decrypt.Api.Models.Exceptions
{
    public class ApiErrorException(HttpStatusCode statusCode, string errorMessage) : Exception(errorMessage)
    {
        public ApiError ApiError => new()
        {
            StatusCode = statusCode,
            Message = Message
        };
    }
}
