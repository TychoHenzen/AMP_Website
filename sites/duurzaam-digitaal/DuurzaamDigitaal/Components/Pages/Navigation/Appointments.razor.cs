#region

using Amp.Data.Entities;
using Amp.Data.Repositories;
using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public partial class Appointments : ComponentBase
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required IAppointmentRepository AppointmentRepository { get; set; }

    protected AppointmentFormModel AppointmentModel { get; set; } = new();
    protected bool IsSubmitting { get; set; }
    protected string? ErrorMessage { get; set; }

    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Afspraak Maken",
        Subtitle = "Plan een afspraak voor reparatie of ondersteuning",
        Buttons = new List<HeroButton>()
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Belangrijk om te weten",
                Description = string.Empty,
                PricingItems = new List<PricingItem>
                {
                    new() { Label = "Gratis diagnose van het probleem" },
                    new() { Label = "No-cure-no-pay garantie bij reparaties" },
                    new() { Label = "Gebruik van duurzame onderdelen waar mogelijk" },
                    new() { Label = "Voorrijkosten gratis binnen 15km" }
                }
            },
            new()
            {
                Title = "Liever telefonisch?",
                Description = "U kunt ons ook direct bellen voor een afspraak:",
                ContactItems = new List<ContactItem>
                {
                    new()
                    {
                        Icon = "bi bi-telephone",
                        Label = "Telefoon",
                        Value = "[Your Phone]",
                        Href = "tel:[Your Phone]"
                    }
                },
                FooterText = "Bereikbaar op werkdagen van 9:00 tot 17:00"
            }
        }
    };

    protected async Task HandleValidSubmit()
    {
        if (IsSubmitting) return;

        try
        {
            IsSubmitting = true;
            ErrorMessage = null;
            StateHasChanged();

            // Create appointment entity
            var appointment = new Appointment
            {
                FirstName = AppointmentModel.FirstName,
                LastName = AppointmentModel.LastName,
                Email = AppointmentModel.Email,
                Phone = AppointmentModel.Phone,
                ServiceType = AppointmentModel.ServiceType,
                Description = AppointmentModel.Description,
                Location = AppointmentModel.Location,
                Status = "Pending",
                TimeSlot = new AppointmentTimeSlot
                {
                    Date = AppointmentModel.PreferredDate!.Value,
                    TimeOfDay = AppointmentModel.PreferredTime
                }
            };

            // Save to database
            var createdAppointment = await AppointmentRepository.CreateAsync(appointment);

            if (createdAppointment?.Id != null)
            {
                NavigationManager.NavigateTo($"/afspraak-bevestiging?id={createdAppointment.Id}");
                return;
            }

            ErrorMessage = "Er is een fout opgetreden bij het maken van de afspraak.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Form submission error: {ex.Message}");
            ErrorMessage = "Er is een fout opgetreden bij het maken van de afspraak. Probeer het later opnieuw.";
        }
        finally
        {
            IsSubmitting = false;
            StateHasChanged();
        }
    }
}