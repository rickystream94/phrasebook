using Microsoft.AspNetCore.Builder;
using PhrasebookBackendService.Middlewares;

namespace PhrasebookBackendService.Web
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseEasyAuthUserValidation(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EasyAuthUserValidationMiddleware>();
        }
    }
}
