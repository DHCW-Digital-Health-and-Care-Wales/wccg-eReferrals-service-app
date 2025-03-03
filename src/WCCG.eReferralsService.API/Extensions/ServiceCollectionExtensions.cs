using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.Configuration;
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
        services.AddScoped<IHeaderValidator, HeaderValidator>();
    }

    public static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IReferralService, ReferralService>((provider, client) =>
        {
            var pasApiConfig = provider.GetRequiredService<IOptions<PasReferralsApiConfig>>().Value;
            client.BaseAddress = new Uri(pasApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(pasApiConfig.TimeoutSeconds);
        });
    }
}
