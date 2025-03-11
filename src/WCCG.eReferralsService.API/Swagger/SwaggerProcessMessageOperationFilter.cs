using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Swagger;

[ExcludeFromCodeCoverage]
public class SwaggerProcessMessageOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var processMessageRequestAttribute = context.MethodInfo.GetCustomAttribute<SwaggerProcessMessageRequestAttribute>();

        if (processMessageRequestAttribute is null)
        {
            return;
        }

        operation.Parameters = [];

        AddHeaders(operation, RequestHeaderKeys.GetAllRequired(), true);
        AddHeaders(operation, RequestHeaderKeys.GetAllOptional(), false);

        AddResponses(operation);
        AddRequests(operation);
    }

    private static void AddHeaders(OpenApiOperation operation, IEnumerable<string> headers, bool isRequired)
    {
        foreach (var header in headers)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                In = ParameterLocation.Header,
                Example = new OpenApiString(RequestHeaderKeys.GetExampleValue(header)),
                Required = isRequired,
                Name = header,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }

    private static void AddRequests(OpenApiOperation operation)
    {
        operation.RequestBody = new OpenApiRequestBody();
        operation.RequestBody.Content.Add(FhirConstants.FhirMediaType,
            new OpenApiMediaType
            {
                Example = new OpenApiString(
                    File.ReadAllText("Swagger/Examples/process-message-payload&response.json"))
            });
    }

    private static void AddResponses(OpenApiOperation operation)
    {
        operation.Responses = new OpenApiResponses
        {
            {
                "200", new OpenApiResponse
                {
                    Description = "OK",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            RequestHeaderKeys.GetExampleValue(RequestHeaderKeys.Accept), new OpenApiMediaType
                            {
                                Example = new OpenApiString(
                                    File.ReadAllText("Swagger/Examples/process-message-payload&response.json")),
                            }
                        }
                    }
                }
            },
            {
                "400", new OpenApiResponse
                {
                    Description = "Bad Request",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            FhirConstants.FhirMediaType, new OpenApiMediaType
                            {
                                Example = new OpenApiString(File.ReadAllText("Swagger/Examples/process-message-bad-request.json")),
                            }
                        }
                    }
                }
            }
        };
    }
}
