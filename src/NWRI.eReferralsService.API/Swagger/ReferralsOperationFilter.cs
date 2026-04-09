using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Nodes;
using JetBrains.Annotations;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using NWRI.eReferralsService.API.Swagger.Attributes;

namespace NWRI.eReferralsService.API.Swagger;

[ExcludeFromCodeCoverage]
[UsedImplicitly(Reason = "Registered in DI")]
public sealed class ReferralsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetCustomAttribute<SwaggerGetReferralByIdRequestAttribute>() is not null)
        {
            ApplyGetReferral(operation);
        }
        else if (context.MethodInfo.GetCustomAttribute<SwaggerGetReferralsRequestAttribute>() is not null)
        {
            ApplyGetReferrals(operation);
        }
    }

    private static void ApplyGetReferral(OpenApiOperation operation)
    {
        SwaggerHelpers.AddCommonHeaders(operation);
        SwaggerHelpers.AddPathParameter(operation, "id", required: true, example: JsonValue.Create(Guid.NewGuid().ToString()));
        SwaggerHelpers.AddProxyNotImplementedResponses(operation);
    }

    private static void ApplyGetReferrals(OpenApiOperation operation)
    {
        SwaggerHelpers.AddCommonHeaders(operation);
        SwaggerHelpers.AddProxyNotImplementedResponses(operation);
    }
}
