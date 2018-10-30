using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Robin.Lottery.WebApp.Middlewares
{
    public class ErrorMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(ILogger<ErrorMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next?.Invoke(context);
            }
            catch (ApplicationException ae)
            {
                _logger.LogError(ae.Message, ae.InnerException);
            }
        }
    }
}