using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Helpers;
using Task = System.Threading.Tasks.Task;

namespace WCCG.eReferralsService.API.Middleware;

public class ResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<ResponseMiddleware> _logger;

    public ResponseMiddleware(RequestDelegate next, JsonSerializerOptions serializerOptions, ILogger<ResponseMiddleware> logger)
    {
        _next = next;
        _serializerOptions = serializerOptions;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        AddResponseHeaders(context);
        AddOperationIdHeader(context);

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        OperationOutcome body;

        switch (exception)
        {
            case MissingRequiredHeaderException missingRequiredHeaderException:
                _logger.RequiredHeadersMissingError(missingRequiredHeaderException);

                statusCode = HttpStatusCode.BadRequest;
                body = OperationOutcomeCreator.CreateOperationOutcome(missingRequiredHeaderException);
                break;

            //todo: Add other cases

            default:
                _logger.UnexpectedError(exception);

                body = OperationOutcomeCreator.CreateOperationOutcome(OperationOutcome.IssueType.Transient, new UnexpectedError(exception));
                break;
        }

        context.Response.ContentType = FhirConstants.FhirMediaType;
        context.Response.StatusCode = (int)statusCode;
        await context.Response.Body.WriteAsync(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body, _serializerOptions)));
    }

    private static void AddResponseHeaders(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(RequestHeaderKeys.RequestId, out var requestId))
        {
            context.Response.Headers.TryAdd(RequestHeaderKeys.RequestId, requestId);
        }

        if (context.Request.Headers.TryGetValue(RequestHeaderKeys.CorrelationId, out var correlationId))
        {
            context.Response.Headers.TryAdd(RequestHeaderKeys.CorrelationId, correlationId);
        }
    }

    private static void AddOperationIdHeader(HttpContext context)
    {
        context.Response.Headers.TryAdd("X-Operation-Id", Activity.Current?.TraceId.ToString());
    }
}
