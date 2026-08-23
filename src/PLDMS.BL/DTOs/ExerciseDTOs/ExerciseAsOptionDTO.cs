namespace PLDMS.BL.DTOs;

public record ExerciseAsOptionDTO
{
	public long Id { get; set; }
	public string Name { get; set; } = null!;
}