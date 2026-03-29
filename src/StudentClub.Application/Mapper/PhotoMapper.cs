using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentClub.Application.Mappings
{
    public class PhotoMapper
    {
        public Photo ToEntity(UploadPhotoRequestDto request, string url, string publicId)
        {
            return new Photo
            {
                Url = url,
                PublicId = publicId,
                Title = request.Title,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow,

                UserId = request.UserId == 0 ? null : request.UserId,
                ClubId = request.ClubId == 0 ? null : request.ClubId,
                EventId = request.EventId == 0 ? null : request.EventId,
                ClubMemberId = request.ClubMemberId == 0 ? null : request.ClubMemberId
            };
        }

        public PhotoResponseDto ToResponse(Photo photo)
        {
            if (photo == null) return null!;

            return new PhotoResponseDto
            {
                PhotoId = photo.PhotoId,
                Url = photo.Url,
                Title = photo.Title,
                Type = photo.Type,
                CreatedAt = photo.CreatedAt,
                UserId = photo.UserId,
                ClubId = photo.ClubId,
                EventId = photo.EventId
            };
        }

        public List<PhotoResponseDto> ToListResponse(IEnumerable<Photo> photos)
        {
            if (photos == null || !photos.Any())
                return new List<PhotoResponseDto>();

            return photos.Select(photo => ToResponse(photo)).ToList();
        }

        public Photo UpdatePhotoEntity(Photo existingPhoto, UpdatePhotoRequestDto request, string? newUrl = null, string? newPublicId = null)
        {
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                existingPhoto.Title = request.Title;
            }

            if (request.Type.HasValue)
            {
                existingPhoto.Type = request.Type.Value;
            }

            if (!string.IsNullOrEmpty(newUrl) && !string.IsNullOrEmpty(newPublicId))
            {
                existingPhoto.Url = newUrl;
                existingPhoto.PublicId = newPublicId;
            }

            return existingPhoto;
        }
    }
}