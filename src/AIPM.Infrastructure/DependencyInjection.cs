using AIPM.Application.Identity;
using AIPM.Application.Runtime.Events;
using AIPM.Application.Runtime.Resilience;
using AIPM.Application.Runtime.Workers;
using AIPM.Infrastructure.AI;
using AIPM.Infrastructure.Configuration;
using AIPM.Infrastructure.Events;
using AIPM.Infrastructure.Identity;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Repositories;
using AIPM.Infrastructure.Messaging;
using AIPM.Infrastructure.Messaging.Consumers;
using AIPM.Infrastructure.Messaging.DeadLetter;
using AIPM.Infrastructure.Messaging.Health;
using AIPM.Infrastructure.Messaging.Idempotency;
using AIPM.Infrastructure.Resilience;
using AIPM.Infrastructure.Workers;
using AIPM.SharedKernel.Execution;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AIPM.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers infrastructure services.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPlatformConfiguration(configuration);
        services.AddAiProviderFoundation();
        services.AddSingleton<IExecutionContextAccessor, AsyncLocalExecutionContextAccessor>();
        services.AddSingleton<IDeadLetterQueue, InMemoryDeadLetterQueue>();
        services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();
        services.AddSingleton<IResilienceExecutor, PollyResilienceExecutor>();
        services.AddSingleton<MessagingPipelineHealthState>();
        services.AddSingleton<IConsumerIdempotencyStore, InMemoryConsumerIdempotencyStore>();
        services.AddSingleton<IMessagingDeadLetterSink, InMemoryMessagingDeadLetterSink>();
        services.AddSingleton<BackgroundWorkerHost>();
        services.AddSingleton<IBackgroundWorkerHost>(sp => sp.GetRequiredService<BackgroundWorkerHost>());
        services.AddHostedService(sp => sp.GetRequiredService<BackgroundWorkerHost>());

        var rabbitConnection = configuration.GetConnectionString("RabbitMq");
        var useInMemoryBus = string.IsNullOrWhiteSpace(rabbitConnection)
            || rabbitConnection.Equals("inmemory", StringComparison.OrdinalIgnoreCase);

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection)
            && !redisConnection.Equals("inmemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnection));
        }

        services.AddMassTransit(x =>
        {
            x.AddConsumer<PlatformStartedConsumer>();
            x.AddConsumer<PlatformHealthConsumer>();

            if (useInMemoryBus)
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMilliseconds(100)));
                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitConnection!));
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMilliseconds(200)));
                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        services.AddScoped<IMessageBus, MassTransitMessageBus>();
        services.AddScoped<IIdentityEventPublisher, IdentityEventPublisher>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        var identityConnection = configuration.GetConnectionString("IdentityDb");
        if (string.IsNullOrWhiteSpace(identityConnection))
        {
            identityConnection = "Data Source=aipm.identity.db";
        }

        services.AddDbContext<IdentityDbContext>(options => options.UseSqlite(identityConnection));

        return services;
    }
}
