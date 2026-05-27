using Microsoft.AspNetCore.Mvc.Diagnostics;
using Scalar.AspNetCore; 
using  APIs.Services; // Imports JobService class with dummy data from the Services folder
 
 //Phase 1 : Builder - Register the services into the app
 /// Dependency injection container
 
 var builder = WebApplication.CreateBuilder(args);

 //Register Your services

 builder.Services.AddControllers(); //registering controller support
 builder.Services.AddOpenApi(); // Registering built-in OpenApi document generation
 builder.Services.AddSingleton<JobServices>(); // Registers JobServices as a Singleton service that will be used as a single instance the entire project


 var app = builder.Build(); //Nothing can be regsitered after this

//Phase 2: Pipeline - Configure your Middleware chain
// NB: Order matters!! 

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}
app.MapControllers(); 

// Returns all available jobs
app.MapGet("/jobs", async (JobServices jobService) =>
{
    var jobs = await jobService.GetAllJobsAsync();

    return Results.Ok(jobs);
});

 // Returns a single job listing by ID
app.MapGet("/jobs/{id}", async (int id, JobServices jobService) =>
{
    var job = await jobService.GetJobByIdAsync(id);

    if (job is null)
    {
        return Results.NotFound(); // Return HTTP 404 status if job does not exist
    }

    return Results.Ok(job); // Return HTTP 200 OK status with the data
});



app.Run(); 