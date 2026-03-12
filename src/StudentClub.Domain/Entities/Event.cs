using StudentClub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace StudentClub.Domain.Entities;

[Table("Events")]
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

    public EventPriority? Priority { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
