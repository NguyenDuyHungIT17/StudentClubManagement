using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IClubMemberRepository
    {
        Task AddClubMemberAsync(ClubMember clubMember);
        Task SaveChangeAsync();

        Task<int> GetClubIdByUserId(int userId);
    }
}
