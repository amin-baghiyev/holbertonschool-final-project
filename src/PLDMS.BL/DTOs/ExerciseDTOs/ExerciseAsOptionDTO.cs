namespace PLDMS.BL.DTOs;

public record ExerciseAsOptionDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}