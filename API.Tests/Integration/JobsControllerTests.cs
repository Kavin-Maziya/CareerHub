using System.Net;
using System.Net.Http.Json;
using APIs.DTOs;
using Xunit;

namespace API.Tests.Integration;

public class JobsControllerTests : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly HttpClient _client;

    public JobsControllerTests(WebApplicationFactoryFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetJobs_ReturnsOk()
    {
        var response = await _client.GetAsync("api/v1/jobs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetJobs_ResponseIsPagedEnvelope()
    {
        var response = await _client.GetFromJsonAsync<PagedResponse<JobListResponse>>("api/v1/jobs?page=1&pageSize=5");
        Assert.NotNull(response);
        Assert.Equal(1, response.Page);
        Assert.Equal(5, response.PageSize);
        Assert.True(response.TotalCount >= 0);
    }

    [Fact]
    public async Task GetJobs_ResponseIncludesXTotalCountHeader()
    {
        var response = await _client.GetAsync("api/v1/jobs");
        Assert.True(response.Headers.Contains("X-Total-Count"));
    }

    [Fact]
    public async Task GetJobs_WithoutVersion_ReturnsSameStatusAsV1()
    {
        var response = await _client.GetAsync("api/jobs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetJobs_ResponseIncludesApiSupportedVersionsHeader()
    {
        var response = await _client.GetAsync("api/v1/jobs");
        Assert.True(response.Headers.Contains("api-supported-versions"));
        var headerValue = response.Headers.GetValues("api-supported-versions").First();
        Assert.Contains("1.0", headerValue);
    }

    [Fact]
    public async Task PostJob_WithoutToken_Returns401Unauthorized()
    {
        var payload = new { Title = "Hacker", CompanyName = "Security Group" };
        var response = await _client.PostAsJsonAsync("api/v1/jobs", payload);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostApplication_WithoutToken_Returns401Unauthorized()
    {
        var payload = new { CoverLetter = "Hire me please." };
        var response = await _client.PostAsJsonAsync("api/v1/applications", payload);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetJobById_WithValidId_DoesNotReturn500InternalServerError()
    {
        var fallbackGuid = Guid.NewGuid();
        var response = await _client.GetAsync($"api/v1/jobs/{fallbackGuid}");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetJobById_ResponseIncludesETagHeader()
    {
        var listResponse = await _client.GetFromJsonAsync<PagedResponse<JobListResponse>>("api/v1/jobs?page=1&pageSize=1");
        if (listResponse != null && listResponse.Data.Any())
        {
            var id = listResponse.Data.First().Id;
            var response = await _client.GetAsync($"api/v1/jobs/{id}");
            Assert.True(response.Headers.ETag != null);
        }
    }

    [Fact]
    public async Task GetJobById_WithMatchingETag_Returns304NotModified()
    {
        var listResponse = await _client.GetFromJsonAsync<PagedResponse<JobListResponse>>("api/v1/jobs?page=1&pageSize=1");
        if (listResponse != null && listResponse.Data.Any())
        {
            var id = listResponse.Data.First().Id;
            var initialResponse = await _client.GetAsync($"api/v1/jobs/{id}");
            var etag = initialResponse.Headers.ETag?.Tag;

            if (!string.IsNullOrEmpty(etag))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/jobs/{id}");
                request.Headers.TryAddWithoutValidation("If-None-Match", etag);
                var secondResponse = await _client.SendAsync(request);
                Assert.Equal(HttpStatusCode.NotModified, secondResponse.StatusCode);
            }
        }
    }
}