using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class CreateEventRegistrationRequestDto
    {
        public int EventId { get; set; }

        public int UserId { get; set; }

        public bool? CheckedIn { get; set; }

    }
}
