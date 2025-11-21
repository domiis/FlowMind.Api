using System.Text.Json;
using FlowMind.Api.Data;
using FlowMind.Api.Services;
using FlowMind.Api.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.SwaggerGen;

// ===============================================
// IDENTIFICA SE ESTÁ EM AMBIENTE DE TESTE
// ===============================================
var isTesting = AppDomain.CurrentDomain.GetAssemblies()
    .Any(a => a.FullName?.Contains("FlowMind.Tests") ?? false);

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. DATABASE (Oracle) – mas permite substituição nos testes
// ============================================================
var oracleConn = builder.Configuration.GetConnectionString("OracleConnection");

// Só registra Oracle se NÃO estiver testando
if (!isTesting)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseOracle(oracleConn);
    });

    // Health check só funciona se Oracle estiver habilitado
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>("OracleDatabase");
}
else
{
    // NO TESTE → registra somente o InMemory no Factory, então não mexe aqui
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("FlowMindTestDb"));
}

// ============================================================
// 2. SERVIÇOS E AUTOMAPPER
// ============================================================
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IIAService, IAServiceMock>();

// ============================================================
// 3. VERSIONAMENTO + SWAGGER
// ============================================================
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

// ============================================================
// 4. OPEN TELEMETRY (TRACING)
// ============================================================
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("FlowMind")
            );
    });

// ============================================================
// 5. LOGGING
// ============================================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ============================================================
// 6. CONTROLLERS + JSON
// ============================================================
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

// ============================================================
// PIPELINE
// ============================================================
if (app.Environment.IsDevelopment() && !isTesting)
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

if (!isTesting)
{
    app.MapHealthChecks("/health");
}

app.Run();

// Necessário para o WebApplicationFactory nos testes
public partial class Program { }
