namespace PLDMS.BL.DTOs.MentorDTOs;

public record MentorTableItemDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}