using System.Text.Json;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.Configuration.OptionValidators;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Middleware;
using WCCG.eReferralsService.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

//PasReferralsApiConfig
builder.Services.AddOptions<PasReferralsApiConfig>().Bind(builder.Configuration.GetSection(PasReferralsApiConfig.SectionName));
builder.Services.AddSingleton<IValidateOptions<PasReferralsApiConfig>, ValidatePasReferralsApiConfigOptions>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => { options.OperationFilter<SwaggerProcessMessageOperationFilter>(); });

builder.Services.AddApplicationInsights(builder.Environment.IsDevelopment(), builder.Configuration);

builder.Services.AddSingleton(new JsonSerializerOptions().ForFhirExtended());
builder.Services.AddHttpClients();
builder.Services.AddValidators();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ResponseMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
