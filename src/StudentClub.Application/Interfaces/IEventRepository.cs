using StudentClub.Domain.Entities;


namespace StudentClub.Application.Interfaces
{
    public interface IEventRepository
    {
        Task AddEventAsync(Event e);
        Task SaveChangeAsync();
        Task<Event?> GetByEventIdAsync(int eventId);
    }
}
