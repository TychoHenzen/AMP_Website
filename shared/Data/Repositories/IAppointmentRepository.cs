#region

using Amp.Data.Entities;

#endregion

namespace Amp.Data.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(string id);
    Task<Appointment> CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(string id);
    Task<IEnumerable<Appointment>> GetByStatusAsync(string status);
    Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync();
    Task UpdateStatusAsync(string id, string status);
}