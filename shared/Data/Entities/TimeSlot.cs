#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace Amp.Data.Entities;

public class TimeSlot : BaseDocument
{
    public TimeSlot() : base("TimeSlot", "timeslot")
    {
    }

    [Required]
    [JsonProperty("startTime")]
    public DateTime StartTime { get; set; }

    [Required]
    [JsonProperty("endTime")]
    public DateTime EndTime { get; set; }

    [JsonProperty("isAvailable")]
    public bool IsAvailable { get; set; } = true;

    [JsonProperty("appointmentId")]
    public string? AppointmentId { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; } = string.Empty;
}