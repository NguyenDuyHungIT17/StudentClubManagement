using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IClubRepository
    {
        Task<Club> GetClubByClubNameAsync (string clubName);
        Task<Club> GetClubByClubIdAsync(int id);
        Task<string?> GetCLubNameByClubIdAsync(int  clubId);
        Task<List<Club>> GetClubsAsync();
        Task<Club> GetClubAsync(int id);
        Task UpdateLeaderIdAsync(int clubId, int leaderId);
        Task UpdateClubAsync (Club club);
        Task AddClubAsync(Club club);
        Task DeleteEventRegistrationsByClubIdAsync(int clubId);
        Task DeleteFeedbacksByClubIdAsync(int clubId);
        Task DeleteEventsByClubIdAsync(int clubId);
        Task DeleteMembersByClubIdAsync(int clubId);
        Task DeleteInterviewsByClubIdAsync(int clubId);
        Task DeleteClubAsync(Club club);
        Task SaveChangeAsync();
    }
}
