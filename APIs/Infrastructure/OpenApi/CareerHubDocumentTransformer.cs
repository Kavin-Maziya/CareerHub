using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace APIs.Infrastructure.OpenApi;

public class CareerHubDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Title = "CareerHub API";
        document.Info.Version = "v1";
        document.Info.Description =
            "The CareerHub JobListings Application that allows job seekers to search and apply for jobs using the plartform. " +
            "Public endpoints allow browsing job listings with no authentication, " +
            "While administrative and write operations require JWT authentication.";

        document.Info.Contact = new OpenApiContact
        {
            Name = "Kavin Maziya",
            Email = "kavinmaziya256@gmail.com",
            Url = new Uri("https://careerhubapi.production.com")
        };

        return Task.CompletedTask;
    }
}