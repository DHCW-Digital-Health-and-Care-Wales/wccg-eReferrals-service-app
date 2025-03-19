using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.RateLimiting;
using Azure.Identity;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.RateLimiting;
using Polly.Retry;
using Polly.Timeout;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Configuration.Resilience;
using WCCG.eReferralsService.API.Models;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.API.Validators;

namespace WCCG.eReferralsService.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddApplicationInsights(this IServiceCollection services, bool isDevelopmentEnvironment, IConfiguration configuration)
    {
        var appInsightsConnectionString = configuration.GetRequiredSection(ApplicationInsightsConfig.SectionName)
            .GetValue<string>(nameof(ApplicationInsightsConfig.ConnectionString));

        services.AddApplicationInsightsTelemetry(options => options.ConnectionString = appInsightsConnectionString);
        services.Configure<TelemetryConfiguration>(config =>
        {
            if (isDevelopmentEnvironment)
            {
                config.SetAzureTokenCredential(new AzureCliCredential());
                return;
            }

            var clientId = configuration.GetRequiredSection(ManagedIdentityConfig.SectionName)
                .GetValue<string>(nameof(ManagedIdentityConfig.ClientId));
            config.SetAzureTokenCredential(new ManagedIdentityCredential(clientId));
        });
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<BundleModel>, BundleModelValidator>();
        services.AddScoped<IValidator<HeadersModel>, HeadersModelValidator>();
    }

    public static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IReferralService, ReferralService>((provider, client) =>
        {
            var pasApiConfig = provider.GetRequiredService<IOptions<PasReferralsApiConfig>>().Value;
            client.BaseAddress = new Uri(pasApiConfig.BaseUrl);
        }).AddResilienceHandler("default", CreateResiliencePipeline);
    }

    private static void CreateResiliencePipeline(
        ResiliencePipelineBuilder<HttpResponseMessage> builder,
        ResilienceHandlerContext context)
    {
        var resilienceConfig = context.ServiceProvider.GetRequiredService<IOptions<ResilienceConfig>>().Value;

        builder
            .AddRateLimiter(new RateLimiterStrategyOptions
            {
                DefaultRateLimiterOptions = new ConcurrencyLimiterOptions
                {
                    PermitLimit = resilienceConfig.RateLimiter.PermitLimit,
                    QueueLimit = resilienceConfig.RateLimiter.QueueLimit
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                BackoffType = resilienceConfig.Retry.IsExponentialDelay
                    ? DelayBackoffType.Exponential
                    : DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds(resilienceConfig.Retry.DelaySeconds),
                UseJitter = true,
                MaxRetryAttempts = resilienceConfig.Retry.MaxRetries,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<TimeoutRejectedException>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response => response.StatusCode
                        is HttpStatusCode.RequestTimeout
                        or HttpStatusCode.TooManyRequests
                        or HttpStatusCode.InternalServerError
                        or HttpStatusCode.BadGateway
                        or HttpStatusCode.ServiceUnavailable
                        or HttpStatusCode.GatewayTimeout)
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = (double)resilienceConfig.CircuitBreaker.FailureRatioPercent / 100,
                MinimumThroughput = resilienceConfig.CircuitBreaker.MinimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(resilienceConfig.CircuitBreaker.SamplingDurationSeconds),
                BreakDuration = TimeSpan.FromSeconds(resilienceConfig.CircuitBreaker.BreakDurationSeconds)
            })
            .AddTimeout(TimeSpan.FromSeconds(resilienceConfig.AttemptTimeoutSeconds));
    }
}
