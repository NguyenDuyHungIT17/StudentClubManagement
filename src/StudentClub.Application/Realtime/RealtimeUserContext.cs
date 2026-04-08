namespace StudentClub.Application.Realtime
{
    /// <summary>
    /// Context chứa thông tin user từ JWT token
    /// </summary>
    public class RealtimeUserContext
    {
        /// <summary>
        /// ID user
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Role (admin, leader, member, guest)
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// ID CLB (nếu user là member của CLB)
        /// </summary>
        public int? ClubId { get; set; }
    }
}