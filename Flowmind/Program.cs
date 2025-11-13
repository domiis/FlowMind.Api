// Program.cs
using System.Text.Json;
using FlowMind.Api.Data;
using FlowMind.Api.Services;
using FlowMind.Api.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using Swashbuckle.AspNetCore.SwaggerGen;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// 1. CONFIGURAÇÃO DO BANCO (ORACLE)
// ===============================
var oracleConn = builder.Configuration.GetConnectionString("OracleConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(oracleConn));

// ===============================
// 2. MONITORAMENTO: HEALTH CHECK
// ===============================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("OracleDatabase");

// ===============================
// 3. AUTO MAPPER E SERVIÇOS
// ===============================
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IIAService, IAServiceMock>();

// ===============================
// 4. SWAGGER + VERSIONAMENTO
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// ===============================
// 5. OPEN TELEMETRY (TRACING)
// ===============================
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FlowMind"));
    });

// ===============================
// 6. LOGGING (CONSOLE + DEBUG)
// ===============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===============================
// 7. IURLHELPER (PARA HATEOAS)
// ===============================
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<Microsoft.AspNetCore.Mvc.IUrlHelper>(sp =>
{
    var httpContext = sp.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().HttpContext!;
    var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
    return new Microsoft.AspNetCore.Mvc.Routing.UrlHelper(actionContext);
});

// ===============================
// 8. CONTROLLERS + JSON OPTIONS
// ===============================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

// ===============================
// PIPELINE
// ===============================
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();