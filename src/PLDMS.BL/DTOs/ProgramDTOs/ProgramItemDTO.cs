namespace PLDMS.BL.DTOs.ProgramDTOs;

public record ProgramItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }
    public bool IsDeleted { get; set; }
}