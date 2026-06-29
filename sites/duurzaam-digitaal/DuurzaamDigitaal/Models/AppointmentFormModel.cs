#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace DuurzaamDigitaal.Models;

public class AppointmentFormModel
{
    [Required(ErrorMessage = "Voornaam is verplicht")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Voornaam moet tussen 2 en 100 karakters zijn")]
    [RegularExpression(@"^[a-zA-Z\s-]*$", ErrorMessage = "Voornaam mag alleen letters bevatten")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Achternaam is verplicht")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Achternaam moet tussen 2 en 100 karakters zijn")]
    [RegularExpression(@"^[a-zA-Z\s-]*$", ErrorMessage = "Achternaam mag alleen letters bevatten")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig email adres")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefoonnummer is verplicht")]
    [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
    [RegularExpression(@"^[0-9\s+()-]*$", ErrorMessage = "Ongeldig telefoonnummer formaat")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecteer een type dienst")]
    public string ServiceType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecteer een datum")]
    public DateTime? PreferredDate { get; set; }

    [Required(ErrorMessage = "Selecteer een tijdvak")]
    [RegularExpression("^(morning|afternoon)$", ErrorMessage = "Selecteer een geldig tijdvak")]
    public string PreferredTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Omschrijving is verplicht")]
    [MinLength(10, ErrorMessage = "Geef een duidelijke omschrijving van minimaal 10 tekens")]
    [MaxLength(1000, ErrorMessage = "Omschrijving mag niet langer zijn dan 1000 tekens")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecteer een locatie")]
    [RegularExpression("^(workshop|home)$", ErrorMessage = "Selecteer een geldige locatie")]
    public string Location { get; set; } = string.Empty;
}