namespace Amp.Data.Nido;

public interface INidoAppointmentRepository
{
    Task<NidoAppointment> CreateAsync(NidoAppointment appointment);

    /// <summary>Non-cancelled bookings for a single date (single-partition query).</summary>
    Task<IReadOnlyList<NidoAppointment>> GetByDateAsync(string date);

    /// <summary>Non-cancelled bookings on/after a date, ordered by date then time (admin view).</summary>
    Task<IReadOnlyList<NidoAppointment>> GetUpcomingAsync(string fromDate);

    /// <summary>Delete a booking (frees the slot). Idempotent — missing doc is a no-op.</summary>
    Task DeleteAsync(string id, string date);
}
