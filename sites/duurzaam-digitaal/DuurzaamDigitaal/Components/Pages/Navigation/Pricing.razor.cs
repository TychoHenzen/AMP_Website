using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public partial class PricingBase : ComponentBase
{
    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Transparante Prijzen",
        Subtitle = "Duidelijke tarieven zonder verborgen kosten en no-cure-no-pay garantie",
        Buttons = new List<HeroButton>
        {
            new()
            {
                Text = "Direct een afspraak maken",
                Href = "/afspraak",
                Icon = "bi bi-calendar"
            }
        }
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Direct Contact",
                Description = "Wilt u meer weten over onze prijzen of heeft u een specifieke vraag? Neem gerust contact met ons op.",
                ContactItems = new List<ContactItem>
                {
                    new()
                    {
                        Icon = "bi bi-telephone",
                        Label = "Telefoon",
                        Value = "[Your Phone]",
                        Href = "tel:[Your Phone]"
                    },
                    new()
                    {
                        Icon = "bi bi-envelope",
                        Label = "Email",
                        Value = "[Your Email]",
                        Href = "mailto:[Your Email]"
                    }
                },
                Buttons = new List<HeroButton>
                {
                    new()
                    {
                        Text = "Afspraak Maken",
                        Href = "/afspraak",
                        Icon = "bi bi-calendar"
                    },
                    new()
                    {
                        Text = "Contact Opnemen",
                        Href = "/contact",
                        Icon = "bi bi-envelope",
                        IsOutline = true
                    }
                }
            }
        }
    };
}
