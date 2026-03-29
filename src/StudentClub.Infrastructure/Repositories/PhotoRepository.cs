using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence; // Thay bằng namespace chứa DbContext của bạn nếu khác
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClub.Infrastructure.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly StudentClubDbContext _context;

        public PhotoRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task AddPhotoAsync(Photo photo)
        {
            await _context.Photos.AddAsync(photo);
        }

        public async Task AddRangeAsync(IEnumerable<Photo> photos)
        {
            await _context.Photos.AddRangeAsync(photos);
        }

        public async Task<Photo?> GetPhotoByIdAsync(int photoId)
        {
            return await _context.Photos
                .FirstOrDefaultAsync(p => p.PhotoId == photoId);
        }

        public async Task<List<Photo>> GetPhotosByEventIdAsync(int eventId)
        {
            return await _context.Photos
                .Where(p => p.EventId == eventId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Photo>> GetPhotosByClubIdAsync(int clubId)
        {
            return await _context.Photos
                .Where(p => p.ClubId == clubId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Photo>> GetPhotosByUserIdAsync(int userId)
        {
            return await _context.Photos
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Photo>> GetPhotosByClubMemberIdAsync(int clubMemberId)
        {
            return await _context.Photos
                .Where(p => p.ClubMemberId == clubMemberId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdatePhotoAsync(Photo photo)
        {
            _context.Photos.Update(photo);
            await Task.CompletedTask;
        }

        public async Task DeletePhotoAsync(Photo photo)
        {
            _context.Photos.Remove(photo);
            await Task.CompletedTask; 
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}