using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClub.Domain.Entities;

[Table("Interviews")]
public partial class Interview
{
    public int InterviewId { get; set; }

    public int ClubId { get; set; }

    public string ApplicantName { get; set; } = null!;

    public string ApplicantEmail { get; set; } = null!;

    public string? Evaluation { get; set; }

    public string? Result { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Club Club { get; set; } = null!;
}
