namespace Amp.Api.Nido;

public record SlotDto(string Time, bool Available);

public record AvailabilityResponse(string Date, bool Open, bool Past, IReadOnlyList<SlotDto> Slots);

public record BookingResponse(string Id, string Status, string Date, string Time);

/// <summary>Incoming booking payload from the Nido site.</summary>
public class BookingRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Service { get; set; }
    public string? Date { get; set; }   // yyyy-MM-dd
    public string? Time { get; set; }   // HH:mm
    public string? Notes { get; set; }
}
