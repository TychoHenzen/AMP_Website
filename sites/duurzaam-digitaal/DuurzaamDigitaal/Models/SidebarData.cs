namespace DuurzaamDigitaal.Models;

public class SidebarData
{
    public List<SidebarSection> Sections { get; set; } = new();
}

public class SidebarSection
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<PricingItem> PricingItems { get; set; } = new();
    public List<ContactItem> ContactItems { get; set; } = new();
    public List<HeroButton> Buttons { get; set; } = new();
    public string FooterText { get; set; } = string.Empty;
}

public class PricingItem
{
    public string Icon { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ContactItem
{
    public string Icon { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}