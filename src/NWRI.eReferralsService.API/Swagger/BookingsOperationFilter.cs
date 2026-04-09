using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Nodes;
using JetBrains.Annotations;
using Microsoft.OpenApi;
using NWRI.eReferralsService.API.Swagger.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NWRI.eReferralsService.API.Swagger;

[ExcludeFromCodeCoverage]
[UsedImplicitly(Reason = "Registered in DI")]
public sealed class BookingsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetCustomAttribute<SwaggerGetAppointmentsRequestAttribute>() is not null)
        {
            ApplyGetAppointments(operation);
        }
        else if (context.MethodInfo.GetCustomAttribute<SwaggerGetBookingSlotRequestAttribute>() is not null)
        {
            ApplyGetBookingSlot(operation);
        }
        else if (context.MethodInfo.GetCustomAttribute<SwaggerGetAppointmentByIdRequestAttribute>() is not null)
        {
            ApplyGetAppointmentById(operation);
        }
    }

    private static void ApplyGetAppointments(OpenApiOperation operation)
    {
        SwaggerHelpers.AddCommonHeaders(operation);
        SwaggerHelpers.AddProxyNotImplementedResponses(operation);
    }

    private static void ApplyGetAppointmentById(OpenApiOperation operation)
    {
        SwaggerHelpers.AddCommonHeaders(operation);
        SwaggerHelpers.AddPathParameter(operation, "id", required: true, example: JsonValue.Create(Guid.NewGuid().ToString()));
        SwaggerHelpers.AddProxyNotImplementedResponses(operation);
    }

    private static void ApplyGetBookingSlot(OpenApiOperation operation)
    {
        SwaggerHelpers.AddCommonHeaders(operation);

        AddBookingSlotQueryParameters(operation);

        SwaggerHelpers.AddProxyNotImplementedResponses(operation);
    }

    private static void AddBookingSlotQueryParameters(OpenApiOperation operation)
    {
        SwaggerHelpers.UpsertParameter(operation, new OpenApiParameter
        {
            Name = "status",
            In = ParameterLocation.Query,
            Required = true,
            Description = "Comma-separated Slot status values (free, busy). Default: free.",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Example = JsonValue.Create("free,busy")
            }
        });

        SwaggerHelpers.UpsertParameter(operation, new OpenApiParameter
        {
            Name = "start",
            In = ParameterLocation.Query,
            Required = true,
            Description = "Use twice with ge and le prefixes to define time window.",
            Style = ParameterStyle.Form,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema { Type = JsonSchemaType.String },
                Example = new JsonArray(
                    "ge2022-03-01T12:00:00+00:00",
                    "le2022-03-01T13:30:00+00:00")
            }
        });

        SwaggerHelpers.UpsertParameter(operation, new OpenApiParameter
        {
            Name = "_include",
            In = ParameterLocation.Query,
            Required = true,
            Description =
                "FHIR _include parameters. Repeat the parameter to include multiple values. " +
                "Minimum required: Slot:schedule and Schedule:actor:HealthcareService. " +
                "Unsupported _include values will be ignored and omitted from the response Bundle.link.url.",
            Style = ParameterStyle.Form,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = new List<JsonNode>
                    {
                        JsonValue.Create("Slot:schedule"),
                        JsonValue.Create("Schedule:actor:Practitioner"),
                        JsonValue.Create("Schedule:actor:PractitionerRole"),
                        JsonValue.Create("Schedule:actor:HealthcareService"),
                        JsonValue.Create("HealthcareService:providedBy"),
                        JsonValue.Create("HealthcareService:location"),
                        JsonValue.Create("Slot:*")
                    }
                }
            },
            Example = new JsonArray(
                "Slot:schedule",
                "Schedule:actor:HealthcareService")
        });
    }
}
