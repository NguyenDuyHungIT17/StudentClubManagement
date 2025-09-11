using System;
using System.Collections.Generic;

namespace StudentClub.Domain.Entities;

public partial class ClubMember
{
    public int ClubMemberId { get; set; }

    public int ClubId { get; set; }

    public int UserId { get; set; }

    public string? MemberRole { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
