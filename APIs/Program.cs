using APIs.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using API.Middleware;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using APIs.Infrastructure;
using APIs.Infrastructure.OpenApi;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.ResponseCompression;
using APIs.Services;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up CareerHub number 1 Job Listing platform...");
    var builder = WebApplication.CreateBuilder(args);


    builder.Host.UseSerilog();


    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });
    builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
    builder.Services.AddRateLimiter(limiterOptions =>
    {
        // Define the custom OnRejected response callback
        limiterOptions.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "text/plain";

            int retryAfterSeconds = 0;
            // Extract window reset duration token from metadata lease
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterSpan))
                retryAfterSeconds = (int)retryAfterSpan.TotalSeconds;

            // Write the required HTTP response header
            context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString();

            // Write the plain text payload
            await context.HttpContext.Response.WriteAsync(
                $"Rate limit exceeded. Please retry after {retryAfterSeconds} seconds.", token);
        };
        // GLOBAL POLICY: Fixed Window (200 requests / 60 seconds)
        limiterOptions.AddFixedWindowLimiter("global", options =>
        {
            options.PermitLimit = 200;
            options.Window = TimeSpan.FromSeconds(60);
            options.QueueLimit = 0; // Reject immediately
        });
        // SEARCH POLICY: Sliding Window (30 requests / 60 seconds)
        limiterOptions.AddSlidingWindowLimiter("search", options =>
        {
            options.PermitLimit = 30;
            options.Window = TimeSpan.FromSeconds(60);
            options.SegmentsPerWindow = 6; // 6 segments checked every 10 seconds
            options.QueueLimit = 0; // Reject immediately
        });
        // APPLY POLICY: Fixed Window (5 requests / 60 minutes)
        limiterOptions.AddFixedWindowLimiter("apply", options =>
        {
            options.PermitLimit = 5;
            options.Window = TimeSpan.FromMinutes(60);
            options.QueueLimit = 0; // Reject immediately
        });

        // POST-LISTING POLICY: Fixed Window (10 requests / 60 minutes)
        limiterOptions.AddFixedWindowLimiter("post-listing", options =>
        {
            options.PermitLimit = 10;
            options.Window = TimeSpan.FromMinutes(60);
            options.QueueLimit = 0; // Reject immediately

        });
    });
    builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<CareerHubDocumentTransformer>();
});
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<CareerHubDbContext>(
            name: "database",
            tags: ["ready"]);

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        
    });

    builder.Services.AddProblemDetails();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CareerHubFrontEndPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://careerhub.production.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("X-Total-Count");
        });
    });

    var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecretKey!)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
        });

    builder.Services.AddAuthorization();

    // AddDatabase now also registers SlowQueryInterceptor (Part 7)
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddApplicationServices();
    builder.Services.AddHostedService<JobListingExpiryService>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseResponseCompression();
    app.UseCors("CareerHubFrontEndPolicy");
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

// /health/live — answers "is the process running?"
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });
// /health/ready — answers "can the process serve traffic?" (include a database check)
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapControllers().RequireRateLimiting("global");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
    

}