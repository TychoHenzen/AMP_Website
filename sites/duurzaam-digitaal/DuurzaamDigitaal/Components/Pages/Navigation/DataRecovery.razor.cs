#region

using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public class DataRecoveryBase : ComponentBase
{
    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Data Recovery",
        Subtitle = "Professionele hulp bij het herstellen van uw belangrijke bestanden",
        Buttons = new List<HeroButton>
        {
            new()
            {
                Text = "Direct hulp nodig?",
                Href = "afspraak",
                Icon = "bi bi-calendar-check"
            }
        }
    };

    protected ServiceCardData IntroCard { get; set; } = new()
    {
        Icon = "bi bi-shield-lock",
        Title = "Professionele Data Recovery Service",
        Description =
            "Bij DuurzaamDigitaal begrijpen we hoe waardevol uw gegevens zijn. Wij zetten alles op alles om uw data veilig te herstellen.",
        Features = new List<ServiceFeature>
        {
            new() { Text = "Gratis initiële diagnose" },
            new() { Text = "No-recovery-no-pay garantie" },
            new() { Text = "Strikte data vertrouwelijkheid" },
            new() { Text = "Beveiligde recovery omgeving" },
            new() { Text = "Transparant recovery proces" }
        }
    };

    protected List<ServiceCardData> SolutionCards { get; set; } = new()
    {
        new ServiceCardData
        {
            Icon = "bi bi-file-earmark-excel",
            Title = "Logische Problemen",
            Description = "Herstel van data bij software-gerelateerde problemen.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Verwijderde bestanden" },
                new() { Text = "Formattering fouten" },
                new() { Text = "Corrupte bestandssystemen" },
                new() { Text = "Virus schade herstel" }
            }
        },
        new ServiceCardData
        {
            Icon = "bi bi-tools",
            Title = "Fysieke Problemen",
            Description = "Specialistische recovery bij hardware defecten.",
            Features = new List<ServiceFeature>
            {
                new() { Text = "Vastgelopen harde schijven" },
                new() { Text = "Elektronische storingen" },
                new() { Text = "Waterschade" },
                new() { Text = "Mechanische defecten" }
            }
        }
    };

    protected ServiceCardData ProcessCard { get; set; } = new()
    {
        Icon = "bi bi-diagram-3",
        Title = "Ons Recovery Process",
        Description = "Transparant en professioneel van diagnose tot oplossing",
        Features = new List<ServiceFeature>
        {
            new() { Text = "Gratis evaluatie van de situatie", Icon = "bi bi-1-circle" },
            new() { Text = "Gedetailleerd onderzoek", Icon = "bi bi-2-circle" },
            new() { Text = "Veilig herstel van data", Icon = "bi bi-3-circle" },
            new() { Text = "Controle van herstelde data", Icon = "bi bi-4-circle" }
        }
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Spoedservice",
                Description = "Direct hulp nodig bij dataverlies? Onze spoedservice staat voor u klaar.",
                FooterText = "Beschikbaar op werkdagen 9:00 - 17:00",
                Buttons = new List<HeroButton>
                {
                    new()
                    {
                        Text = "Bel Direct voor Spoedhulp",
                        Href = "tel:[Your Phone]",
                        Icon = "bi bi-telephone-fill"
                    }
                }
            },
            new()
            {
                Title = "Tarieven",
                Description = "Onze recovery diensten zijn verdeeld in twee categorieën:",
                PricingItems = new List<PricingItem>
                {
                    new() { Icon = "bi bi-search", Label = "Diagnose", Price = "Gratis" },
                    new()
                    {
                        Icon = "bi bi-laptop", Label = "Basis recovery", Price = "vanaf €75",
                        Description = "Voor verwijderde bestanden, formattering fouten en virusschade"
                    },
                    new()
                    {
                        Icon = "bi bi-tools", Label = "Complexe recovery", Price = "Op aanvraag",
                        Description = "Voor fysieke schade, vastgelopen schijven en waterschade"
                    },
                    new() { Icon = "bi bi-lightning", Label = "Spoedservice", Price = "+50% toeslag" }
                },
                FooterText = "No-recovery-no-pay garantie van toepassing"
            },
            new()
            {
                Title = "Direct Contact",
                Description = "Data verloren? Neem direct contact op voor hulp.",
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
                        Href = "afspraak",
                        Icon = "bi bi-calendar-check"
                    }
                }
            }
        }
    };
}