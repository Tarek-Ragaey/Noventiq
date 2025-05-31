using Noventiq.Application.IServices.Models.Common;

namespace Noventiq.API.Middleware
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;
        private const string DefaultLanguage = "en";

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if there's a language in the query parameters
            var paginationParams = context.Request.Query.ContainsKey("languageKey")
                ? new PaginationParams { LanguageKey = context.Request.Query["languageKey"].ToString() }
                : null;

            // If no language in query params, check headers
            if (paginationParams == null || string.IsNullOrEmpty(paginationParams.LanguageKey))
            {
                var language = context.Request.Headers.TryGetValue("Accept-Language", out var headerLanguage)
                    ? headerLanguage.ToString()
                    : DefaultLanguage;

                // Store the language in HttpContext.Items for use in controllers
                context.Items["LanguageKey"] = language;
            }

            await _next(context);
        }
    }
}
