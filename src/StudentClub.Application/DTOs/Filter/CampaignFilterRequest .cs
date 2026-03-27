namespace StudentClub.Application.DTOs.Filter
{
    public class CampaignFilterRequest : BaseFilter
    {
        public int? ClubId { get; set; }
        public string? KeyWord { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDesc { get; set; } = true;
    }
}