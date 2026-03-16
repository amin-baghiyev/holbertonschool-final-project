using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record StudentReviewListItemDTO
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = null!;
    public string AssignedByName { get; set; } = null!;
    public int Score { get; set; }
    public ReviewStatus ReviewStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
