using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Filter
{
    public class UserFilterRequest : BaseFilter
    {
        public string? KeyWord { get; set; }
        public string? Role { get; set; }
        
    }
}
