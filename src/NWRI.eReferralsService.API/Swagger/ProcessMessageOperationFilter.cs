using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NWRI.eReferralsService.API.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;
using NWRI.eReferralsService.API.Swagger.Attributes;

namespace NWRI.eReferralsService.API.Swagger;

[ExcludeFromCodeCoverage]
public sealed class ProcessMessageOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetCustomAttribute<SwaggerProcessMessageRequestAttribute>() is not null)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
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
            Content =
            {
                [FhirConstants.FhirMediaType] = new OpenApiMediaType
                {
                    Example = new OpenApiString(
                        File.ReadAllText("Swagger/Examples/process-message-payload.json"))
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
                            Examples = new Dictionary<string, OpenApiExample>
                            {
                                ["proxy-server-error"] = new()
                                {
                                    Summary = "Proxy error",
                                    Value = new OpenApiString(File.ReadAllText("Swagger/Examples/common-proxy-server-error.json"))
                                },
                                ["receiver-server-error"] = new()
                                {
                                    Summary = "WPAS API error",
                                    Value = new OpenApiString(File.ReadAllText("Swagger/Examples/common-external-server-error.json"))
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
