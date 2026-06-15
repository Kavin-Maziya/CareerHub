using APIs.Data;
using Microsoft.EntityFrameworkCore;

namespace APIs.Services;

public class JobListingExpiryService(
    IServiceScopeFactory scopeFactory,
    ILogger<JobListingExpiryService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Job Listing Expiry Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Checking for expired job listings at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CareerHubDbContext>();

                var now = DateTime.UtcNow;

                // Find active jobs that have passed their closing date
                var expiredJobs = await db.JobListings
                    .Where(j => j.IsActive && j.ClosingDate <= now)
                    .ToListAsync(stoppingToken);

                if (expiredJobs.Count > 0)
                {
                    foreach (var job in expiredJobs)
                    {
                        job.IsActive = false;
                    }

                    await db.SaveChangesAsync(stoppingToken);
                    logger.LogInformation("Successfully closed {Count} expired job listings.", expiredJobs.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while cleaning up job listings.");
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        logger.LogInformation("Job Listing Cleanup Service is stopping.");
    }
}