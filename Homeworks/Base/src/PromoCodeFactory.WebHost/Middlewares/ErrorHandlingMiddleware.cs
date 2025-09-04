using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using PromoCodeFactory.WebHost.ExceptionHandling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PromoCodeFactory.WebHost.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Handled exception");

                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var error = new ErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Code = ex.Code,
                    Message = ex.Message,
                    Details = _env.IsDevelopment() ? ex.StackTrace : "",
                };

                await context.Response.WriteAsJsonAsync(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var error = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message,
                    Details = _env.IsDevelopment() ? ex.StackTrace : ""
                };

                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
