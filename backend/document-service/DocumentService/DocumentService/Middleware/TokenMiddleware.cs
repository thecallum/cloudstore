using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var tokenService = new TokenService();

            try
            {
                var tokenValue = httpContext.Request.Headers["authorizationToken"];

                var payload = tokenService.DecodeToken(tokenValue);

                httpContext.Items["user"] = payload;

                await _next(httpContext);
            } catch(Exception)
            {
                httpContext.Items["user"] = null;

                await _next(httpContext);
            }
        }
    }

   // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }
}
