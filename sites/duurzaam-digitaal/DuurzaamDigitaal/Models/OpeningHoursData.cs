namespace DuurzaamDigitaal.Models;

public class OpeningHoursData
{
    private const string STANDARD_OPENING = "09:00";
    private const string STANDARD_CLOSING = "17:00";

    public Dictionary<DayOfWeek, BusinessHours> RegularHours { get; set; } = new()
    {
        { DayOfWeek.Monday, new BusinessHours { IsOpen = false, EmergencyOnly = true } },
        { DayOfWeek.Tuesday, new BusinessHours { OpenTime = STANDARD_OPENING, CloseTime = STANDARD_CLOSING } },
        { DayOfWeek.Wednesday, new BusinessHours { OpenTime = STANDARD_OPENING, CloseTime = STANDARD_CLOSING } },
        { DayOfWeek.Thursday, new BusinessHours { OpenTime = STANDARD_OPENING, CloseTime = STANDARD_CLOSING } },
        { DayOfWeek.Friday, new BusinessHours { OpenTime = STANDARD_OPENING, CloseTime = STANDARD_CLOSING } },
        { DayOfWeek.Saturday, new BusinessHours { OpenTime = STANDARD_OPENING, CloseTime = STANDARD_CLOSING } },
        { DayOfWeek.Sunday, new BusinessHours { IsOpen = false } }
    };
}

public class BusinessHours
{
    public bool IsOpen { get; set; } = true;
    public string? OpenTime { get; set; }
    public string? CloseTime { get; set; }
    public bool EmergencyOnly { get; set; }

    public string GetDisplayText()
    {
        if (!IsOpen) return EmergencyOnly ? "Closed (Emergency appointments only)" : "Closed";
        return OpenTime != null && CloseTime != null ? $"{OpenTime} - {CloseTime}" : "Hours not set";
    }
}