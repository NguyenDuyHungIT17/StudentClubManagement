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
        Task DeleteEvent(int id);
        Task<List<Event>> GetPublicEventsByCLubIdAsync(int clubId, bool check);
        Task<Event?> GetEventByIdAsync(int eventId);

        Task<string> GetEventNameByIdAsync(int eventId);
        //Task<List<Event>> GetEventsByUserIdAsync(int userId);  eventregister 
        Task<Event?> GetByEventIdAsync(int eventId);
    }
}
