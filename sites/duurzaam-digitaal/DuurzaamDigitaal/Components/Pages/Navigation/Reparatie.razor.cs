#region

using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public class ReparatieBase : ComponentBase
{
    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Computer Reparatie",
        Subtitle = "Professionele reparatie met no-cure-no-pay garantie. Uw technologie verdient een tweede kans.",
        Buttons = new List<HeroButton>
        {
            new()
            {
                Text = "Direct een afspraak maken",
                Href = "afspraak",
                Icon = "bi bi-calendar-check"
            },
            new()
            {
                Text = "Bekijk onze tarieven",
                Href = "prijzen",
                Icon = "bi bi-currency-euro",
                IsOutline = true
            }
        }
    };

    protected ServiceCardData GuaranteeCard { get; set; } = new()
    {
        Icon = "bi bi-shield-check",
        Title = "Onze No-Cure-No-Pay Garantie",
        Description = "U betaalt alleen als we het probleem daadwerkelijk kunnen oplossen.",
        Features = new List<ServiceFeature>
        {
            new() { Text = "Gratis uitgebreide diagnose" },
            new() { Text = "Transparante prijsopgave vooraf" },
            new() { Text = "Geen verrassingen achteraf" },
            new() { Text = "Garantie op alle reparaties" }
        }
    };

    protected List<ServiceCardData> ServiceCards { get; set; } = new()
    {
        new ServiceCardData
        {
            Icon = "bi bi-laptop",
            Title = "Hardware Reparatie",
            Description = "Professionele reparatie van laptops en computers met gebruik van duurzame onderdelen.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Scherm reparatie" },
                new() { Text = "Toetsenbord vervanging" },
                new() { Text = "Batterij vervangen" },
                new() { Text = "Opladers vervangen" }
            }
        },
        new ServiceCardData
        {
            Icon = "bi bi-gear",
            Title = "Preventief Onderhoud",
            Description = "Voorkom problemen met regelmatig onderhoud en tijdige updates van uw systeem.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Interne reiniging" },
                new() { Text = "Thermal paste vervanging" },
                new() { Text = "Hardware controle" },
                new() { Text = "Systeem optimalisatie" }
            }
        }
    };

    protected ServiceCardData ProcessCard { get; set; } = new()
    {
        Icon = "bi bi-arrow-repeat",
        Title = "Ons Reparatieproces",
        Description = "Transparant en effectief van diagnose tot oplossing",
        Features = new List<ServiceFeature>
        {
            new() { Text = "Gratis probleemanalyse", Icon = "bi bi-1-circle" },
            new() { Text = "Duidelijk kostenvoorstel", Icon = "bi bi-2-circle" },
            new() { Text = "Vakkundige uitvoering", Icon = "bi bi-3-circle" },
            new() { Text = "Uitgebreide testing", Icon = "bi bi-4-circle" }
        }
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Tarieven",
                PricingItems = new List<PricingItem>
                {
                    new() { Icon = "bi bi-search", Label = "Diagnose", Price = "Gratis" },
                    new() { Icon = "bi bi-clock", Label = "Arbeidsloon", Price = "€45 per uur" },
                    new() { Icon = "bi bi-truck", Label = "Voorrijkosten", Price = "Gratis binnen 15km" }
                },
                FooterText = "* Betaling alleen bij succesvolle reparatie",
                Buttons = new List<HeroButton>
                {
                    new()
                    {
                        Text = "Bekijk alle tarieven",
                        Href = "prijzen",
                        Icon = "bi bi-list-ul"
                    }
                }
            },
            new()
            {
                Title = "Direct Contact",
                Description = "Heeft u een dringende reparatie nodig?",
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
                        Text = "Maak een afspraak",
                        Href = "afspraak",
                        Icon = "bi bi-calendar-check"
                    },
                    new()
                    {
                        Text = "Stel een vraag",
                        Href = "contact",
                        Icon = "bi bi-chat-dots",
                        IsOutline = true
                    }
                }
            }
        }
    };
}