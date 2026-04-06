using System.ComponentModel.DataAnnotations;

namespace PLDMS.BL.DTOs;

public record MentorAssignReviewDTO
{
    [Required]
    public Guid GroupId { get; init; }

    [Required(ErrorMessage = "Please select a student to review.")]
    public Guid ReviewerId { get; init; }
}
