using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace APIs.Infrastructure;

// Part 7: Logs any database command that exceeds SlowQueryThresholdMs.
// Registered as Singleton because it holds no request state — only reads
// from configuration which is also a singleton.
public class SlowQueryInterceptor(IConfiguration configuration, ILogger<SlowQueryInterceptor> logger)
    : DbCommandInterceptor
{
    private int ThresholdMs =>
        configuration.GetValue<int?>("SlowQueryThresholdMs") ?? 100;

    // Async hook — covers the vast majority of EF Core queries in this project
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(eventData.Duration, command.CommandText);
        return result;
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(eventData.Duration, command.CommandText);
        return new ValueTask<DbDataReader>(result);
    }

    private void LogIfSlow(TimeSpan duration, string sql)
    {
        if (duration.TotalMilliseconds >= ThresholdMs)
        {
            logger.LogWarning(
                "Slow query detected ({ElapsedMs}ms >= threshold {ThresholdMs}ms):\n{Sql}",
                (int)duration.TotalMilliseconds,
                ThresholdMs,
                sql);
        }
    }
}
