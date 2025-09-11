using System;
using System.Collections.Generic;

namespace StudentClub.Domain.Entities;

public partial class EventRegistration
{
    public int RegistrationId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public bool? CheckedIn { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
