using StudentClub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Domain.Entities.Realtime
{
    public class ChatMessage
    {
        public Guid Id { get; private set; }
        public Guid? FromUserId { get; private set; }
        public Guid? ToUserId { get; private set; }
        public Guid ClubId { get; private set; }
        public ChatScope Scope { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }

    }
}
