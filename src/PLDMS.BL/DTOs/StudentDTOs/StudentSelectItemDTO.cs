namespace PLDMS.BL.DTOs;

public record StudentSelectItemDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}