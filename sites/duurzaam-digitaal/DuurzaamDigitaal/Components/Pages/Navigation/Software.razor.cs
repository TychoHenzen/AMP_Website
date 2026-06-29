#region

using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public class SoftwareBase : ComponentBase
{
    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Software Support",
        Subtitle = "Persoonlijke computerhulp en training op uw eigen tempo",
        Buttons = new List<HeroButton>
        {
            new()
            {
                Text = "Direct een afspraak maken",
                Href = "afspraak",
                Icon = "bi bi-calendar-check"
            }
        }
    };

    protected List<ServiceCardData> MainCards { get; set; } = new()
    {
        new ServiceCardData
        {
            Icon = "bi bi-mortarboard",
            Title = "Training voor Senioren",
            Description = "Persoonlijke begeleiding op uw eigen tempo met duidelijke uitleg en veel geduld.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Basis computergebruik" },
                new() { Text = "Email en internet" },
                new() { Text = "Video bellen" }
            }
        },
        new ServiceCardData
        {
            Icon = "bi bi-shield-check",
            Title = "Beveiliging & Onderhoud",
            Description =
                "Bescherm uw computer tegen virussen en andere dreigingen met onze complete beveiligingsservice.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Virus en malware verwijdering" },
                new() { Text = "Antivirus software installatie" },
                new() { Text = "Windows updates" },
                new() { Text = "Back-up instellingen" },
                new() { Text = "Internet beveiliging" }
            }
        }
    };

    protected List<ServiceCardData> ApproachCards { get; set; } = new()
    {
        new ServiceCardData
        {
            Icon = "bi bi-person-check",
            Title = "Wat we bieden",
            Description = "Persoonlijke ondersteuning afgestemd op uw leertempo en behoeften.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Persoonlijke één-op-één uitleg" },
                new() { Text = "Duidelijke instructies op papier" },
                new() { Text = "Geduldig en in uw tempo" },
                new() { Text = "Praktische oefeningen" },
                new() { Text = "Nazorg en vervolgvragen" }
            }
        },
        new ServiceCardData
        {
            Icon = "bi bi-tools",
            Title = "Extra services",
            Description = "Technische ondersteuning voor al uw computer- en apparaatbehoeften.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Software installatie hulp" },
                new() { Text = "Printer installatie" },
                new() { Text = "Email configuratie" },
                new() { Text = "Cloud opslag instellen" },
                new() { Text = "Smartphone synchronisatie" }
            }
        }
    };

    protected ServiceCardData SecurityCard { get; set; } = new()
    {
        Icon = "bi bi-shield-lock",
        Title = "Last van een trage computer of verdachte pop-ups?",
        Description = "Wij kunnen helpen!",
        Features = new List<ServiceFeature>
        {
            new() { Text = "Grondige systeemcontrole" },
            new() { Text = "Verwijdering van virussen en malware" },
            new() { Text = "Preventieve maatregelen" },
            new() { Text = "Advies over veilig internetgebruik" },
            new() { Text = "Back-up van belangrijke bestanden" },
            new() { Text = "Beveiligingssoftware installatie" }
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
                    new() { Icon = "bi bi-chat-dots", Label = "Basis consult", Price = "€35 per uur" },
                    new() { Icon = "bi bi-laptop", Label = "Training sessies", Price = "€40 per uur" },
                    new() { Icon = "bi bi-shield", Label = "Virus verwijdering", Price = "€60 vast tarief" },
                    new() { Icon = "bi bi-download", Label = "Software installatie", Price = "vanaf €35" },
                    new() { Icon = "bi bi-windows", Label = "Windows installatie", Price = "€75" }
                },
                FooterText = "Voorrijkosten gratis binnen 15km"
            },
            new()
            {
                Title = "Waarom DuurzaamDigitaal?",
                Description = "Onze unieke aanpak",
                PricingItems = new List<PricingItem>
                {
                    new() { Label = "Persoonlijke en geduldige aanpak" },
                    new() { Label = "Duidelijke uitleg zonder vakjargon" },
                    new() { Label = "Flexibele afspraakmogelijkheden" },
                    new() { Label = "Ervaren met seniorentraining" },
                    new() { Label = "Nazorg en ondersteuning" }
                }
            },
            new()
            {
                Title = "Contact",
                Description = "Heeft u vragen of wilt u een afspraak maken?",
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
                        Text = "Neem contact op",
                        Href = "contact",
                        Icon = "bi bi-chat-dots",
                        IsOutline = true
                    }
                }
            }
        }
    };
}