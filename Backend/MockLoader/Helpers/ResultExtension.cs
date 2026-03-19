using FluentResults;

namespace MockLoader.Helpers
{
    /// <summary>
    /// Helper to get from the failed Fluent.Result the first error message
    /// </summary>
    internal static class ResultExtension
    {
        public static string GetMessage(this Result result)
        {
            var errorMessage = "Error message not found";

            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault();

                if (error != null)
                {
                    errorMessage = error.Message;
                }
            }

            return errorMessage;
        }

        public static string GetMessage<T>(this Result<T> result)
        {
            var errorMessage = "Error message not found";

            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault();

                if (error != null)
                {
                    errorMessage = error.Message;
                }
            }

            return errorMessage;
        }
    }
}