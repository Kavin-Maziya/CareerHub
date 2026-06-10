using Testcontainers.PostgreSql;
using Xunit;

namespace API.Tests.Repository;

public class PostgreSqlContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("CareerHubTest")
        .WithUsername("postgres")
        .WithPassword("password123")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync() => await Container.StartAsync();

    public async Task DisposeAsync() => await Container.DisposeAsync();
}