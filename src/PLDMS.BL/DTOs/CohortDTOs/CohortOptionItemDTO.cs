namespace PLDMS.BL.DTOs;

public record CohortOptionItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}