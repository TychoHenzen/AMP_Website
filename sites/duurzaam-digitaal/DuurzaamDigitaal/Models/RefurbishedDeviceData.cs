namespace DuurzaamDigitaal.Models;

public class RefurbishedDeviceData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Specifications { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public SystemPerformanceData Performance { get; set; } = new();
    public string Condition { get; set; } = string.Empty;
    public string Warranty { get; set; } = "6 maanden garantie";
    public bool IsAvailable { get; set; } = true;
    public string Category { get; set; } = string.Empty; // e.g., "Basic", "Mid", "High"
}