using Microsoft.AspNetCore.Http;
using StudentClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudentClub.Application.DTOs.request
{
    public class UploadPhotoRequestDto
    {
        [Required(ErrorMessage = "Vui lòng chọn file ảnh")]
        public IFormFile File { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề ảnh")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public PhotoType Type { get; set; } // 1: Main, 2: Cover, 3: Side

        public int? UserId { get; set; }
        public int? ClubId { get; set; }
        public int? EventId { get; set; }
        public int? ClubMemberId { get; set; }
        public int? CampaignsId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing photo (partial update allowed)
    /// Either Title or File (or both) can be provided
    /// </summary>
    public class UpdatePhotoRequestDto
    {
        /// <summary>
        /// New title for the photo (optional)
        /// If not provided, existing title is kept
        /// </summary>
        [MaxLength(255, ErrorMessage = "Tiêu đề ảnh không được vượt quá 255 ký tự")]
        public string? Title { get; set; }

        /// <summary>
        /// New photo type/category (optional)
        /// 1: Main, 2: Cover, 3: Side
        /// If not provided, existing type is kept
        /// </summary>
        public PhotoType? Type { get; set; }

        /// <summary>
        /// New image file to replace the current one (optional)
        /// If provided, old image is deleted from Cloudinary
        /// If not provided, current image URL is kept
        /// </summary>
        public IFormFile? File { get; set; }
    }
}