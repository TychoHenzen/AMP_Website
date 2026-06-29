namespace Amp.Api.Nido;

/// <summary>
/// Nido Suave opening rules + slot generation. MVP: fixed hours, hourly slots.
/// Tweak here when real availability rules arrive (per-day hours, breaks, durations).
/// </summary>
public static class NidoSchedule
{
    private static readonly HashSet<DayOfWeek> OpenDays = new()
    {
        DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
    };

    private const int OpenHour = 9;    // first slot 09:00
    private const int CloseHour = 17;  // last slot starts 16:00

    public static bool IsOpen(DateOnly date) => OpenDays.Contains(date.DayOfWeek);

    public static IReadOnlyList<string> Slots(DateOnly date)
    {
        if (!IsOpen(date)) return Array.Empty<string>();
        var slots = new List<string>();
        for (var h = OpenHour; h < CloseHour; h++)
            slots.Add($"{h:D2}:00");
        return slots;
    }

    /// <summary>Current time in the Netherlands (Azure runs UTC).</summary>
    public static DateTime NlNow()
    {
        foreach (var id in new[] { "Europe/Amsterdam", "W. Europe Standard Time" })
        {
            try
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(id));
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return DateTime.UtcNow;
    }
}
