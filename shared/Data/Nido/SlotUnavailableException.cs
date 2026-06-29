namespace Amp.Data.Nido;

/// <summary>
/// Thrown when a slot is already reserved — i.e. an atomic create lost the race because the
/// deterministic slot id already exists. Maps to HTTP 409 at the API.
/// </summary>
public class SlotUnavailableException : Exception
{
    public SlotUnavailableException(string date, string time)
        : base($"Slot {date} {time} is already booked.") { }
}
