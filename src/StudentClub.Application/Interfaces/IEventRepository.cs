using StudentClub.Domain.Entities;


namespace StudentClub.Application.Interfaces
{
    public interface IEventRepository
    {
        Task AddEventAsync(Event e);
        Task SaveChangeAsync();
        Task <List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetEventsByCLubIdAsync(int clubId);
        Task<List<Event>> GetPublicEventsAsync(bool check);
        Task<Event?> GetByEventIdAsync(int eventId);
    }
}
