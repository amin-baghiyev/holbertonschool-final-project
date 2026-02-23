namespace PLDMS.Core.Entities;

public class StudentGroup
{
    public Guid StudentId { get; set; }
    public AppUser Student { get; set; }
    
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
}