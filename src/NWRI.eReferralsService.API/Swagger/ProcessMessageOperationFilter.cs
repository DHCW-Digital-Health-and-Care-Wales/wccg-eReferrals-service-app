using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Nodes;
using JetBrains.Annotations;
using Microsoft.OpenApi;
using NWRI.eReferralsService.API.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;
using NWRI.eReferralsService.API.Swagger.Attributes;

namespace NWRI.eReferralsService.API.Swagger;

[ExcludeFromCodeCoverage]
[UsedImplicitly(Reason = "Registered in DI")]
public sealed class ProcessMessageOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetCustomAttribute<SwaggerProcessMessageRequestAttribute>() is not null)
        {
            operation.Parameters ??= new List<IOpenApiParameter>();
            operation.Parameters.Clear();

            SwaggerHelpers.AddCommonHeaders(operation);

            AddRequestBody(operation);
            AddResponses(operation);
        }
    }

    private static void AddRequestBody(OpenApiOperation operation)
    {
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [FhirConstants.FhirMediaType] = new()
                {
                    Examples = new Dictionary<string, IOpenApiExample>
                    {
                        ["create"] = new OpenApiExample
                        {
                            Summary = "Create referral",
                            Value = JsonValue.Create(
                                File.ReadAllText("Swagger/Examples/process-message-create-payload.json"))
                        },
                        ["cancel"] = new OpenApiExample
                        {
                            Summary = "Cancel referral",
                            Value = JsonValue.Create(
                                File.ReadAllText("Swagger/Examples/process-message-cancel-payload.json"))
                        }
                    }
                }
            }
        };
    }

    private static void AddResponses(OpenApiOperation operation)
    {
        operation.Responses = new OpenApiResponses
        {
            ["200"] = SwaggerHelpers.CreateFhirResponseWithExample(
                "OK",
                "Swagger/Examples/process-message-ok-response.json"),
            ["400"] = SwaggerHelpers.CreateFhirResponseWithExample(
                "Bad Request",
                "Swagger/Examples/process-message-bad-request.json"),
            ["429"] = SwaggerHelpers.CreateFhirResponseWithExample(
                "Too many requests",
                "Swagger/Examples/common-too-many-requests.json"),
            ["500"] = new OpenApiResponse
            {
                Description = "Internal Server Error",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [FhirConstants.FhirMediaType] = new()
                    {
                        Examples = new Dictionary<string, IOpenApiExample>
                        {
                            ["proxy-server-error"] = new OpenApiExample
                            {
                                Summary = "Proxy error",
                                Value = JsonValue.Create(File.ReadAllText("Swagger/Examples/common-proxy-server-error.json"))
                            },
                            ["receiver-server-error"] = new OpenApiExample
                            {
                                Summary = "WPAS API error",
                                Value = JsonValue.Create(File.ReadAllText("Swagger/Examples/common-external-server-error.json"))
                            }
                        }
                    }
                }
            },
            ["503"] = SwaggerHelpers.CreateFhirResponseWithExample(
                "Service Unavailable",
                "Swagger/Examples/common-service-unavailable.json")
        };
    }
}
