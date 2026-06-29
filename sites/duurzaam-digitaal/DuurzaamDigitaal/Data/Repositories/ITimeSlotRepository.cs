#region

using DuurzaamDigitaal.Data.Entities;

#endregion

namespace DuurzaamDigitaal.Data.Repositories;

public interface ITimeSlotRepository
{
    Task<IEnumerable<TimeSlot>> GetAllAsync();
    Task<TimeSlot?> GetByIdAsync(string id);
    Task<TimeSlot> CreateAsync(TimeSlot timeSlot);
    Task UpdateAsync(TimeSlot timeSlot);
    Task DeleteAsync(string id);
    Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(DateTime startDate, DateTime endDate);
    Task<bool> ReserveTimeSlotAsync(string id, string appointmentId);
    Task ReleaseTimeSlotAsync(string id);
    Task<IEnumerable<TimeSlot>> GetTimeSlotsByLocationAsync(string location);
}