using StudentClub.Application.DTOs;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IFeedbackRepository
    {
        Task CreateFeedbackAsync(Feedback feedbackDto);
        Task<Feedback> GetFeedbackByIdAsync(int feedbackId);
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<List<Feedback>> GetFeedbacksByEventIdAsync(int eventId);
        Task UpdateFeedbackAsync(Feedback feedback);
        Task DeleteFeedbackAsync(int feedbackId);
        Task SaveChangesAsync();
    }
}
