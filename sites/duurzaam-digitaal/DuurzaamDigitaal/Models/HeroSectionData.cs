namespace DuurzaamDigitaal.Models;

public class HeroSectionData
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public List<HeroButton> Buttons { get; set; } = new();
}

public class HeroButton
{
    public string Text { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsOutline { get; set; }
}
