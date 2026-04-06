namespace PLDMS.BL.DTOs;

public record ReviewSummaryDTO
{
    public int TotalReviews { get; init; }
    public int PendingReviews { get; init; }
    public int CompletedReviews { get; init; }
    public int GroupsNeedingReview { get; init; }
}
