using System;
using System.Collections.Generic;

namespace StudentClub.Domain.Entities;

public partial class Club
{
    public int ClubId { get; set; }

    public string ClubName { get; set; } = null!;

    public string? Description { get; set; }

    public int? LeaderId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ClubMember> ClubMembers { get; set; } = new List<ClubMember>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual User? Leader { get; set; }
}
