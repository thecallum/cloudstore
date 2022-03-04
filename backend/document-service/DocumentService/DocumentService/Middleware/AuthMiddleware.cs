using Amazon.Lambda.Core;
using DocumentService.Domain;
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
        private readonly ITokenService _tokenService;

        private readonly HashSet<string> _excludedEndpoints = new HashSet<string>();

        public AuthMiddleware(RequestDelegate next, ITokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;

            _excludedEndpoints.Add("/");
            _excludedEndpoints.Add("/api/auth/login");
            _excludedEndpoints.Add("/api/auth/register");
        }

        public async Task Invoke(HttpContext httpContext)
        {
          LambdaLogger.Log("Calling TokenMiddleware Invoke");

            if (httpContext.GetEndpoint() == null)
            {
                // endpoint doesnt exist
                await _next(httpContext);
                return;
            }

            if (RouteExcluded(httpContext.Request.Path))
            {
                // continue
                await _next(httpContext);
                return;
            }

            string rawToken = httpContext.Request.Headers["Authorization"];
            var payload = _tokenService.DecodeToken(rawToken);

            if (payload == null)
            {
                httpContext.Response.StatusCode = 401; //UnAuthorized
                return;
            }

            // save decoded token to context
            httpContext.Items["user"] = payload;

            await _next(httpContext);
        }

        private bool RouteExcluded(string path)
        {
            return _excludedEndpoints.Contains(path);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
