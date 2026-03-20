using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IUserContext
    {
        int UserId { get; }
        string Role { get; }
    }
}
