using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Domain.Enums;
using StudentClub.Infrastructure.Persistence;
using System;
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

        public async Task<List<Photo>> GetPhotosByCampaignIdAsync(int campaignId)
        {
            return await _context.Photos
                .Where(p => p.CampaignsId == campaignId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // NEW: single main photo (prioritize Main then CreatedAt DESC)
        public async Task<Photo?> GetMainPhotoAsync(int? userId, int? clubId, int? eventId, int? clubMemberId, int? campaignId)
        {
            var query = _context.Photos.AsQueryable();

            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);
            else if (clubId.HasValue)
                query = query.Where(p => p.ClubId == clubId.Value);
            else if (eventId.HasValue)
                query = query.Where(p => p.EventId == eventId.Value);
            else if (clubMemberId.HasValue)
                query = query.Where(p => p.ClubMemberId == clubMemberId.Value);
            else if (campaignId.HasValue)
                query = query.Where(p => p.CampaignsId == campaignId.Value);
            else
                return null;

            return await query
                .OrderBy(p => p.Type == PhotoType.Main ? 0 : 1)
                .ThenByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }

        // NEW: batch main photos for users
        public async Task<Dictionary<int, Photo?>> GetMainPhotosByUserIdsAsync(List<int> userIds)
        {
            var result = userIds.ToDictionary(id => id, id => (Photo?)null);
            if (userIds == null || userIds.Count == 0) return result;

            var photos = await _context.Photos
                .Where(p => p.UserId.HasValue && userIds.Contains(p.UserId.Value))
                .OrderBy(p => p.UserId)
                .ThenBy(p => p.Type == PhotoType.Main ? 0 : 1)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            foreach (var id in userIds)
            {
                var first = photos.FirstOrDefault(p => p.UserId == id);
                result[id] = first;
            }

            return result;
        }

        // NEW: batch main photos for clubs
        public async Task<Dictionary<int, Photo?>> GetMainPhotosByClubIdsAsync(List<int> clubIds)
        {
            var result = clubIds.ToDictionary(id => id, id => (Photo?)null);
            if (clubIds == null || clubIds.Count == 0) return result;

            var photos = await _context.Photos
                .Where(p => p.ClubId.HasValue && clubIds.Contains(p.ClubId.Value))
                .OrderBy(p => p.ClubId)
                .ThenBy(p => p.Type == PhotoType.Main ? 0 : 1)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            foreach (var id in clubIds)
            {
                var first = photos.FirstOrDefault(p => p.ClubId == id);
                result[id] = first;
            }

            return result;
        }

        // NEW: batch main photos for events
        public async Task<Dictionary<int, Photo?>> GetMainPhotosByEventIdsAsync(List<int> eventIds)
        {
            var result = eventIds.ToDictionary(id => id, id => (Photo?)null);
            if (eventIds == null || eventIds.Count == 0) return result;

            var photos = await _context.Photos
                .Where(p => p.EventId.HasValue && eventIds.Contains(p.EventId.Value))
                .OrderBy(p => p.EventId)
                .ThenBy(p => p.Type == PhotoType.Main ? 0 : 1)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            foreach (var id in eventIds)
            {
                var first = photos.FirstOrDefault(p => p.EventId == id);
                result[id] = first;
            }

            return result;
        }

        // NEW: batch main photos for club members
        public async Task<Dictionary<int, Photo?>> GetMainPhotosByClubMemberIdsAsync(List<int> clubMemberIds)
        {
            var result = clubMemberIds.ToDictionary(id => id, id => (Photo?)null);
            if (clubMemberIds == null || clubMemberIds.Count == 0) return result;

            var photos = await _context.Photos
                .Where(p => p.ClubMemberId.HasValue && clubMemberIds.Contains(p.ClubMemberId.Value))
                .OrderBy(p => p.ClubMemberId)
                .ThenBy(p => p.Type == PhotoType.Main ? 0 : 1)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            foreach (var id in clubMemberIds)
            {
                var first = photos.FirstOrDefault(p => p.ClubMemberId == id);
                result[id] = first;
            }

            return result;
        }

        // NEW: batch main photos for campaigns
        public async Task<Dictionary<int, Photo?>> GetMainPhotosByCampaignIdsAsync(List<int> campaignIds)
        {
            var result = campaignIds.ToDictionary(id => id, id => (Photo?)null);
            if (campaignIds == null || campaignIds.Count == 0) return result;

            var photos = await _context.Photos
                .Where(p => p.CampaignsId.HasValue && campaignIds.Contains(p.CampaignsId.Value))
                .OrderBy(p => p.CampaignsId)
                .ThenBy(p => p.Type == PhotoType.Main ? 0 : 1)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            foreach (var id in campaignIds)
            {
                var first = photos.FirstOrDefault(p => p.CampaignsId == id);
                result[id] = first;
            }

            return result;
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

        public async Task DeletePhotosByUserIdAsync(int userId)
        {
            var photos = await _context.Photos
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (photos.Count == 0) return;

            _context.Photos.RemoveRange(photos);
        }

        public async Task DeletePhotosByClubIdAsync(int clubId)
        {
            var photos = await _context.Photos
                .Where(p => p.ClubId == clubId)
                .ToListAsync();

            if (photos.Count == 0) return;

            _context.Photos.RemoveRange(photos);
        }

        public async Task DeletePhotosByEventIdAsync(int eventId)
        {
            var photos = await _context.Photos
                .Where(p => p.EventId == eventId)
                .ToListAsync();

            if (photos.Count == 0) return;

            _context.Photos.RemoveRange(photos);
        }

        public async Task DeletePhotosByClubMemberIdAsync(int clubMemberId)
        {
            var photos = await _context.Photos
                .Where(p => p.ClubMemberId == clubMemberId)
                .ToListAsync();

            if (photos.Count == 0) return;

            _context.Photos.RemoveRange(photos);
        }

        public async Task DeletePhotosByCampaignIdAsync(int campaignId)
        {
            var photos = await _context.Photos
                .Where(p => p.CampaignsId == campaignId)
                .ToListAsync();

            if (photos.Count == 0) return;

            _context.Photos.RemoveRange(photos);
        }

        public async Task DeletePhotoByAnyway(int anyId, int type)
        {
            var photos = type switch
            {
                1 => await _context.Photos.Where(p => p.UserId == anyId).ToListAsync(),
                2 => await _context.Photos.Where(p => p.ClubId == anyId).ToListAsync(),
                3 => await _context.Photos.Where(p => p.EventId == anyId).ToListAsync(),
                4 => await _context.Photos.Where(p => p.ClubMemberId == anyId).ToListAsync(),
                5 => await _context.Photos.Where(p => p.CampaignsId == anyId).ToListAsync(),
                _ => new List<Photo>()
            };
            if (photos.Count == 0) return;
            _context.Photos.RemoveRange(photos);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}