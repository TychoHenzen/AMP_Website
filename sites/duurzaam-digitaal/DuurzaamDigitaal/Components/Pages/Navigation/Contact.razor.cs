#region

using System.ComponentModel.DataAnnotations;
using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public partial class ContactBase : ComponentBase
{
    protected ContactFormModel contactForm = new()
    {
        Name = string.Empty,
        Email = string.Empty,
        Phone = string.Empty,
        Subject = string.Empty,
        Message = string.Empty
    };

    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Contact",
        Subtitle = "Heeft u een vraag of wilt u meer informatie? Neem gerust contact met ons op.",
        Buttons = new List<HeroButton>()
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Contact Informatie",
                Description = "U kunt ons op verschillende manieren bereiken.",
                ContactItems = new List<ContactItem>
                {
                    new()
                    {
                        Icon = "bi bi-geo-alt",
                        Label = "Adres",
                        Value = "[Your Address]",
                        Href = "#"
                    },
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
                    },
                    new()
                    {
                        Icon = "bi bi-whatsapp",
                        Label = "WhatsApp",
                        Value = "[Your Phone]",
                        Href = "https://wa.me/[Your Phone without +]"
                    }
                }
            },
            new()
            {
                Title = "Openingstijden",
                Description =
                    "<ul><li>Dinsdag t/m Zaterdag: 9:00 - 17:00</li><li>Maandag: Alleen spoedgevallen op afspraak</li><li>Zondag: Gesloten</li></ul>"
            },
            new()
            {
                Title = "Spoedgevallen",
                Description =
                    "Heeft u een dringende computerreparatie nodig? Bel ons direct voor spoedhulp tijdens kantooruren.",
                Buttons = new List<HeroButton>
                {
                    new()
                    {
                        Text = "Bel voor spoedhulp",
                        Href = "tel:[Your Phone]",
                        Icon = "bi bi-telephone"
                    }
                }
            }
        }
    };

    protected void HandleValidSubmit()
    {
        // TODO: Implement form submission logic
        // This would typically involve calling an API endpoint
        // For now, we'll just reset the form
        contactForm = new ContactFormModel
        {
            Name = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            Subject = string.Empty,
            Message = string.Empty
        };
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Vul uw naam in")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Vul uw e-mailadres in")]
        [EmailAddress(ErrorMessage = "Vul een geldig e-mailadres in")]
        public required string Email { get; set; }

        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kies een onderwerp")]
        public required string Subject { get; set; }

        [Required(ErrorMessage = "Vul uw bericht in")]
        [MinLength(10, ErrorMessage = "Uw bericht moet minimaal 10 karakters bevatten")]
        public required string Message { get; set; }

        [Required(ErrorMessage = "U moet akkoord gaan met de privacyverklaring")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "U moet akkoord gaan met de privacyverklaring")]
        public bool AcceptPrivacy { get; set; }
    }
}