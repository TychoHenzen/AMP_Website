namespace DuurzaamDigitaal.Models;

public class ServiceCardData
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ServiceFeature> Features { get; set; } = new();
    public bool IsCentered { get; set; } = true;
}

public class ServiceFeature
{
    public string Text { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi bi-check-circle-fill text-success";
}
