using StudentClub.Domain.Enums;

namespace StudentClub.Domain.Entities.Realtime
{
    public class ChatMessage
    {
        public int Id { get; private set; }
        public int? FromUserId { get; private set; }
        public int? ToUserId { get; private set; }
        public int ClubId { get; private set; }
        public ChatScope Scope { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }

    }
}
