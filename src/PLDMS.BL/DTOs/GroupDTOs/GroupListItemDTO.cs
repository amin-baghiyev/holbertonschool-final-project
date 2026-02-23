namespace PLDMS.BL.DTOs;

public record GroupListItemDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<StudentListItemDTO> Students { get; set; } = [];
}