#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace Amp.Data.Entities;

public class Appointment : BaseDocument
{
    public Appointment() : base("Appointment", "appointment")
    {
    }

    [Required]
    [StringLength(100)]
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [JsonProperty("serviceType")]
    public string ServiceType { get; set; } = string.Empty;

    [Required]
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonProperty("location")]
    public string Location { get; set; } = string.Empty;

    [JsonProperty("timeSlot")]
    public AppointmentTimeSlot TimeSlot { get; set; } = new();

    [JsonProperty("status")]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

    [JsonProperty("adminNotes")]
    public string? AdminNotes { get; set; }

    [JsonProperty("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [JsonProperty("cancelledAt")]
    public DateTime? CancelledAt { get; set; }
}

public class AppointmentTimeSlot
{
    [JsonProperty("date")]
    public DateTime Date { get; set; }
    
    [JsonProperty("timeOfDay")]
    public string TimeOfDay { get; set; } = string.Empty;
}
