using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace DocumentService.Infrastructure
{
    public static class RedisInitilisationExtension
    {
        public static IConnectionMultiplexer ConfigureRedis(this IServiceCollection services)
        {
            var configuration = Environment.GetEnvironmentVariable("REDIS_CONFIG") ??
                Environment.GetEnvironmentVariable("REDIS_URL") ??
                "localhost";

            var multiplexer = ConnectionMultiplexer.Connect(configuration, new Action<ConfigurationOptions>(x =>
            {
                x.ResolveDns = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null;
            }));

            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            return multiplexer;
        }
    }
}
