// 
//════════════════════════════════════════════════════ 
// Bootstrap Serilog before the host is built. 
// This ensures even startup exceptions are logged. 
// 
//════════════════════════════════════════════════════ 
using API.Middleware;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using APIs.Data;
using Microsoft.EntityFrameworkCore;
using APIs.Services;
using APIs.Infrastructure;

Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.CreateLogger();
try
{
    Log.Information("Starting up CareerHub number 1 Job Listing platform...");
    var builder = WebApplication.CreateBuilder(args);
    // Replace the default .NET logger with Serilog 
    builder.Host.UseSerilog();
    // 
    //════════════════════════════════════════════════════ 
    // BUILDER — Register services 
    // 
    //════════════════════════════════════════════════════ 
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // Day 3 — typed handler
    builder.Services.AddProblemDetails();

    builder.Services.AddCors(options =>
   {
       options.AddPolicy("FrontEndPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
             .AllowAnyHeader()
             .AllowAnyMethod();
        });
   });

    var jwtSecretKey = builder.Configuration["Jwt:Key"]; //Reads JWT key from appsettings.Development
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // Keep claim types as-is
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Not validating who issues it bc its our own API
            ValidateAudience = false, // Not checking who it is intended for
            ValidateLifetime = true, // This ensures you are able to reject expired tokens
            ValidateIssuerSigningKey = true,// verify the signature
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey!)
            ),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

    builder.Services.AddAuthorization(); //Required for [Authorize(Roles= ...)]
                                         //builder.Services.AddScoped<IAuthService, AuthService>();

    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddApplicationServices();

    //     builder.Services.AddDbContext<CareerHubDbContext>(options =>
    // {
    //     options.UseNpgsql(
    //         builder.Configuration.GetConnectionString("DefaultConnection"));
    // }); //Registers DB context

    //════════════════════════════════════════════════════ 
    // TRANSITION — Build() seals the DI container. 
    // Nothing can be registered after this line. 
    // 
    //════════════════════════════════════════════════════ 
    var app = builder.Build();
    // 
    //════════════════════════════════════════════════════ 
    // PIPELINE — Configure the middleware chain. 
    // Order matters. Top to bottom. 
    // 
    //════════════════════════════════════════════════════ 
    app.UseSerilogRequestLogging(); // Logs every HTTP request + final response automatically 
    app.UseCors("FrontEndPolicy");// Must be early to enable interception of browser preflight options requests
    app.UseExceptionHandler();  // Activates GlobalExceptionHandler — catches all thrown exceptions 
    app.UseStatusCodePages();   // Fills empty 4xx/5xx responses with Problem Details body 

    app.UseAuthentication();
    app.UseAuthorization();
    if (app.Environment.IsDevelopment())
    {
    }
    app.MapOpenApi();
    // Serves /openapi/v1.json 
    app.MapScalarApiReference();  // Serves the Scalar UI at /scalar/v1 
    app.MapControllers();  // Activates attribute routing for all [ApiController] classes 
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start correctly.");
}

finally
{
    Log.CloseAndFlush(); //Ensure all buffered log entries are flushed before application exit. 
}




// builder.Services.AddOpenApi();
// builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

// builder.Services.AddAuthorizationBuilder();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.MapScalarApiReference(option => {
//         option
//             .WithTitle("Auth API")
//             .WithTheme(ScalarTheme.DeepSpace)
//             .WithDownloadButton(true)
//             .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
//     });
// }

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapGet("/test", () => "Hello World!").RequireAuthorization();

// app.Run();
