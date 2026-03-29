using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mappings; // Nhúng Mapper vào đây
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClub.Application.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        private readonly PhotoMapper _photoMapper;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<PhotoService> _logger;


        public PhotoService(
            IPhotoRepository photoRepository,
            IClubRepository clubRepository,
            IUserRepository userRepository,
            PhotoMapper photoMapper,
            IConfiguration config,
            ILogger<PhotoService> logger)
        {
            _photoRepository = photoRepository;
            _clubRepository = clubRepository;
            _userRepository = userRepository;
            _photoMapper = photoMapper;
            _logger = logger;

            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ApiResponse<PhotoResponseDto>> UploadPhotoAsync(UploadPhotoRequestDto request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return ApiResponse<PhotoResponseDto>.Failure(400, "Vui lòng chọn file ảnh hợp lệ.");

                using var stream = request.File.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(request.File.FileName, stream),
                    Folder = "StudentClub/Photos"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return ApiResponse<PhotoResponseDto>.Failure(500, "Lỗi Cloudinary: " + uploadResult.Error.Message);
                }

                var photoEntity = _photoMapper.ToEntity(request, uploadResult.SecureUrl.ToString(), uploadResult.PublicId);

                await _photoRepository.AddPhotoAsync(photoEntity);
                await _photoRepository.SaveChangesAsync();

                var responseDto = _photoMapper.ToResponse(photoEntity);

                return ApiResponse<PhotoResponseDto>.Success(responseDto, "Tải ảnh lên thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload ảnh.");
                return ApiResponse<PhotoResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeletePhotoAsync(int photoId)
        {
            try
            {
                var photo = await _photoRepository.GetPhotoByIdAsync(photoId);
                if (photo == null)
                    return ApiResponse.Failure(404, "Không tìm thấy ảnh.");

                // Xóa trên Cloudinary
                if (!string.IsNullOrEmpty(photo.PublicId))
                {
                    var deleteParams = new DeletionParams(photo.PublicId);
                    var result = await _cloudinary.DestroyAsync(deleteParams);
                    if (result.Result != "ok" && result.Result != "not found")
                    {
                        _logger.LogWarning("Không thể xóa ảnh trên Cloud. PublicId: {PublicId}", photo.PublicId);
                    }
                }

                // Xóa trong Database
                await _photoRepository.DeletePhotoAsync(photo);
                await _photoRepository.SaveChangesAsync();

                return ApiResponse.Success("Xóa ảnh thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ảnh Id: {PhotoId}", photoId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByEventIdAsync(int eventId)
        {
            try
            {
                var photos = await _photoRepository.GetPhotosByEventIdAsync(eventId);
                if (photos == null || !photos.Any())
                    return ApiResponse<List<PhotoResponseDto>>.Failure(404, "Sự kiện này chưa có ảnh.");

                var responseList = _photoMapper.ToListResponse(photos);

                return ApiResponse<List<PhotoResponseDto>>.Success(responseList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy ảnh sự kiện");
                return ApiResponse<List<PhotoResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByClubIdAsync(int clubId)
        {
            try
            {
                var photos = await _photoRepository.GetPhotosByClubIdAsync(clubId);
                if (photos == null || !photos.Any())
                    return ApiResponse<List<PhotoResponseDto>>.Failure(404, "Câu lạc bộ này chưa có ảnh.");
                return ApiResponse<List<PhotoResponseDto>>.Success(_photoMapper.ToListResponse(photos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy ảnh CLB");
                return ApiResponse<List<PhotoResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByUserIdAsync(int userId)
        {
            try
            {
                var photos = await _photoRepository.GetPhotosByUserIdAsync(userId);
                if (photos == null || !photos.Any())
                    return ApiResponse<List<PhotoResponseDto>>.Failure(404, "Người dùng này chưa có ảnh.");

                return ApiResponse<List<PhotoResponseDto>>.Success(_photoMapper.ToListResponse(photos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy ảnh User");
                return ApiResponse<List<PhotoResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<PhotoResponseDto>> UpdatePhotoAsync(int photoId, UpdatePhotoRequestDto request, int userId)
        {
            try
            {
                // 1. Fetch photo
                var photo = await _photoRepository.GetPhotoByIdAsync(photoId);
                if (photo == null)
                {
                    return ApiResponse<PhotoResponseDto>.Failure(404, "Không tìm thấy ảnh.");
                }

                // 2. Check authorization (user is owner, admin, or leader of club)
                var isAuthorized = await IsUserAuthorizedToUpdatePhotoAsync(userId, photo);
                if (!isAuthorized)
                {
                    return ApiResponse<PhotoResponseDto>.Failure(403, "Bạn không có quyền sửa ảnh này.");
                }

                // 3. Handle file upload if provided
                string? newUrl = null;
                string? newPublicId = null;

                if (request.File != null && request.File.Length > 0)
                {
                    // Upload new image to Cloudinary
                    using var stream = request.File.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(request.File.FileName, stream),
                        Folder = "StudentClub/Photos"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        return ApiResponse<PhotoResponseDto>.Failure(500, "Lỗi Cloudinary: " + uploadResult.Error.Message);
                    }

                    newUrl = uploadResult.SecureUrl.ToString();
                    newPublicId = uploadResult.PublicId;

                    // Delete old image from Cloudinary
                    if (!string.IsNullOrEmpty(photo.PublicId))
                    {
                        var deleteParams = new DeletionParams(photo.PublicId);
                        var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
                        if (deleteResult.Result != "ok" && deleteResult.Result != "not found")
                        {
                            _logger.LogWarning("Không thể xóa ảnh cũ trên Cloud. PublicId: {PublicId}", photo.PublicId);
                        }
                    }
                }

                // 4. Update photo entity with new data
                var updatedPhoto = _photoMapper.UpdatePhotoEntity(photo, request, newUrl, newPublicId);

                // 5. Save to database
                await _photoRepository.UpdatePhotoAsync(updatedPhoto);
                await _photoRepository.SaveChangesAsync();

                // 6. Return updated DTO
                var responseDto = _photoMapper.ToResponse(updatedPhoto);

                return ApiResponse<PhotoResponseDto>.Success(responseDto, "Cập nhật ảnh thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ảnh. PhotoId: {PhotoId}, UserId: {UserId}", photoId, userId);
                return ApiResponse<PhotoResponseDto>.Failure(500, ex.Message);
            }
        }
        public async Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByClubMemberIdAsync(int clubMemberId)
        {
            try
            {
                var photos = await _photoRepository.GetPhotosByClubMemberIdAsync(clubMemberId);
                if (photos == null || !photos.Any())
                    return ApiResponse<List<PhotoResponseDto>>.Failure(404, "Thành viên câu lạc bộ này chưa có ảnh.");

                return ApiResponse<List<PhotoResponseDto>>.Success(_photoMapper.ToListResponse(photos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy ảnh theo ClubMemberId: {ClubMemberId}", clubMemberId);
                return ApiResponse<List<PhotoResponseDto>>.Failure(500, ex.Message);
            }
        }
        private async Task<bool> IsUserAuthorizedToUpdatePhotoAsync(int userId, Photo photo)
        {
            // Get user to check role
            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (user == null) return false;

            // Admin can update any photo
            if (user.Role == "admin")
                return true;

            // User can update their own photo
            if (photo.UserId == userId)
                return true;

            // Leader can update photos of their club
            if (photo.ClubId.HasValue)
            {
                var club = await _clubRepository.GetClubByClubIdAsync(photo.ClubId.Value);
                if (club != null && club.LeaderId == userId)
                    return true;
            }

            return false;
        }
    }
}