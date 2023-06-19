using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Email;

public interface ULimiter
{
    /// <summary>
    /// Ensure that limits and it right state and check if limit allow to send new email.
    /// </summary>
    bool IsLimitAllow();

    void IncrementLimiter();
}

public class MonthLimiter : ULimiter
{
    private readonly long monthLimit;
    private long monthUsed = 0;

    /// <summary>
    /// Date of last month limit reset. We store it in long to use Interlocked.
    /// Represent DateTime Ticks.
    /// </summary>
    private long lastMonthReset;

    public MonthLimiter(DateTime lastMonthReset, long monthLimit)
    {
        this.monthLimit = monthLimit;
        this.lastMonthReset = lastMonthReset.Ticks;
    }

    public void IncrementLimiter() => Interlocked.Increment(ref monthUsed);

    public bool IsLimitAllow()
    {
        DateTime now = DateTime.Now;

        // because it's long fractional part is cut
        // so we have 0, 1, 2, ...
        long monthsBetween = (now.Ticks - lastMonthReset) / (TimeSpan.TicksPerDay * 30);

        // 1, 2, 3
        if (monthsBetween > 0)
        {
            long ticksToAdd = monthsBetween * 30 * TimeSpan.TicksPerDay;
            long newResetDate = lastMonthReset + ticksToAdd;

            Interlocked.Exchange(ref lastMonthReset, newResetDate);
            Interlocked.Exchange(ref monthUsed, 0);
        }

        return monthUsed < monthLimit;
    }
}

public class DayLimiter : ULimiter
{
    private readonly int dayLimit;
    private int dayUsed = 0;

    private long lastReset;

    public DayLimiter(int dayLimit)
    {
        this.dayLimit = dayLimit;
        lastReset = DateTime.Today.Ticks;
    }

    public void IncrementLimiter() => Interlocked.Increment(ref dayUsed);

    public bool IsLimitAllow()
    {
        DateTime now = DateTime.Now;

        if (now.Ticks - lastReset > TimeSpan.TicksPerDay)
        {
            Interlocked.Exchange(ref dayUsed, 0);
            Interlocked.Exchange(ref lastReset, now.Date.Ticks);
        }

        return dayUsed < dayLimit;
    }
}