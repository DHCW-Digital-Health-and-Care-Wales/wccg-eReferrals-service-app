using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.ApiClients;
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

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IReferralService, ReferralService>();
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IHeaderValidator, HeaderValidator>();
    }

    public static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options => { options.DefaultApiVersion = new ApiVersion(1, 0); })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    public static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IPasReferralsApiClient, PasReferralsApiClient>((provider, client) =>
        {
            var pasApiConfig = provider.GetRequiredService<IOptions<PasReferralsApiConfig>>().Value;
            client.BaseAddress = new Uri(pasApiConfig.BaseUrl);
        });
    }
}
