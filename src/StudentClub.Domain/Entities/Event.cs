using System;
using System.Collections.Generic;
namespace StudentClub.Domain.Entities;

public partial class Event
{
    public int EventId { get; set; }

    public int ClubId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? EventDate { get; set; }

    public bool? IsPrivate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
