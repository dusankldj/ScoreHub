using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IEventDetailsApiService
    {
        Task<EventDetailsDTO> GetEventDetailsAsync(long eventId, CancellationToken ct = default);
    }
}
