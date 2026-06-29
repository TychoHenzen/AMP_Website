#region

using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public class FaqBase : ComponentBase
{
    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Veelgestelde Vragen",
        Subtitle = "Vind snel antwoorden op de meest voorkomende vragen over onze diensten, prijzen en werkwijze.",
        Buttons = new List<HeroButton>
        {
            new()
            {
                Text = "Contact Opnemen",
                Href = "/contact",
                Icon = "phone",
                IsOutline = false
            }
        }
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Hulp Nodig?",
                Description = "Staat uw vraag er niet tussen? Neem gerust contact met ons op voor persoonlijk advies.",
                Buttons = new List<HeroButton>
                {
                    new()
                    {
                        Text = "Contact Opnemen",
                        Href = "/contact",
                        Icon = "phone",
                        IsOutline = false
                    }
                }
            }
        }
    };
}