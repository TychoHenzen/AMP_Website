namespace DuurzaamDigitaal.Models;

public class SystemPerformanceData
{
    public int GraphicsScore { get; set; }
    public int ProcessorScore { get; set; }
    public int MemoryScore { get; set; }
    public int StorageScore { get; set; }
    public int OverallScore => (GraphicsScore + ProcessorScore + MemoryScore + StorageScore) / 4;

    public static string GetPerformanceTier(int score)
    {
        return score switch
        {
            >= 80 => "Excellent for latest AAA games at high settings",
            >= 60 => "Good for most modern games at medium settings",
            >= 40 => "Suitable for esports and older titles",
            _ => "Basic computing and light gaming"
        };
    }

    public static List<string> GetPerformanceExamples(int score)
    {
        return score switch
        {
            >= 80 => new List<string>
            {
                "4K gaming at high framerates",
                "Ray tracing enabled gaming",
                "Virtual reality ready",
                "Professional 3D rendering"
            },
            >= 60 => new List<string>
            {
                "1440p gaming at stable framerates",
                "Modern games at medium-high settings",
                "Streaming while gaming",
                "Content creation"
            },
            >= 40 => new List<string>
            {
                "1080p gaming at stable framerates",
                "Esports titles at competitive settings",
                "Light video editing",
                "Multiple monitor support"
            },
            _ => new List<string>
            {
                "Basic gaming at lower settings",
                "Office applications",
                "Web browsing",
                "Media consumption"
            }
        };
    }
}