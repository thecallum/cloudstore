using Amazon.Lambda.Core;
using DocumentService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var tokenService = new TokenService();

            LambdaLogger.Log("Calling TokenMiddleware Invoke");

            try
            {
                var tokenValue = httpContext.Request.Headers["Authorization"];

                LambdaLogger.Log("tokenValue: " + tokenValue);


                var payload = tokenService.DecodeToken(tokenValue);

                httpContext.Items["user"] = payload;

                LambdaLogger.Log("Payload decoded: " + payload);


                await _next(httpContext);
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Exception user is null");
                LambdaLogger.Log(e.Message);


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
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
