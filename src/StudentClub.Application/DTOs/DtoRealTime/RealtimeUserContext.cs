using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.DtoRealTime
{
    public class RealtimeUserContext
    {
        public int? UserId { get; set; }   
        public int? ClubId { get; set; }
        public string Role { get; set; } = default!;
    }
}
