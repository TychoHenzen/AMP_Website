using Amp.Data.Entities;
using Newtonsoft.Json;

namespace Amp.Data.Nido;

/// <summary>
/// A booking request for Nido Suave. Partitioned by <see cref="Date"/> (yyyy-MM-dd) so a single
/// day's bookings live in one partition — cheap availability lookups per date.
/// </summary>
public class NidoAppointment : BaseDocument
{
    public NidoAppointment() : base("NidoAppointment", string.Empty) { }

    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("email")] public string Email { get; set; } = string.Empty;
    [JsonProperty("phone")] public string Phone { get; set; } = string.Empty;
    [JsonProperty("service")] public string Service { get; set; } = string.Empty;

    /// <summary>yyyy-MM-dd. Doubles as the Cosmos partition key.</summary>
    [JsonProperty("date")] public string Date { get; set; } = string.Empty;

    /// <summary>HH:mm (24h) slot start.</summary>
    [JsonProperty("time")] public string Time { get; set; } = string.Empty;

    [JsonProperty("notes")] public string Notes { get; set; } = string.Empty;
    [JsonProperty("status")] public string Status { get; set; } = "pending";
}
