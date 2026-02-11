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
        public int ClubId { get; set; }
        public int? ToUserId { get; set; }
        public string Content { get; set; } = default!;
    }
}
