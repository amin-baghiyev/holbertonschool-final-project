namespace PLDMS.BL.DTOs;

public record StudentTableItemDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}