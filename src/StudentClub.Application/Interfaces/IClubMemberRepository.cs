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

        Task<List<ClubMember>> GetAllClubMemberAsync();
        Task<ClubMember> GetClubMemberByIdAsync(int id);
        Task<ClubMember> UpdateClubMemberAsync(ClubMember clubMember);
        Task<int> GetClubIdByUserId(int userId);
    }
}
