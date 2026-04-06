using System.ComponentModel.DataAnnotations;

namespace PLDMS.BL.DTOs;

public record MentorReviewCreateDTO
{
    [Required]
    public Guid GroupId { get; init; }

    [Required(ErrorMessage = "Score is required.")]
    [Range(0, 10, ErrorMessage = "Score must be between 0 and 10.")]
    public int Score { get; init; }

    [Required(ErrorMessage = "Note is required.")]
    [MaxLength(500, ErrorMessage = "Note can't be longer than 500 characters.")]
    public string Note { get; init; } = string.Empty;
}
