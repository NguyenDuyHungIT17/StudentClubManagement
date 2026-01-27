using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.DtoRealTime
{
    public class ChatCommand
    {
        public string Type { get; set; } = default!;
        public Guid ClubId { get; set; }
        public Guid? ToUserId { get; set; }
        public string Content { get; set; } = default!;
    }
}
