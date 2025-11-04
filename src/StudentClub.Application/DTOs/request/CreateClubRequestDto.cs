using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.request
{
    public class CreateClubRequestDto
    {
        public string ClubName {  get; set; }
        public string Description { get; set; }

    }
}
