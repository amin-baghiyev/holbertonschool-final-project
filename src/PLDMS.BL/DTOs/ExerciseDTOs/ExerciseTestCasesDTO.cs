namespace PLDMS.BL.DTOs;

public record ExerciseTestCasesDTO
{
    public string Input { get; set; } = null!;
    public string Output { get; set; } = null!;
    public bool IsExample { get; set; }
}