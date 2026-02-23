namespace PLDMS.BL.DTOs;

public record StudentListItemDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
}