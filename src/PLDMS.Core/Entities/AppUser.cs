using Microsoft.AspNetCore.Identity;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Review>? ReviewsGiven { get; set; }
    public ICollection<Review>? ReviewsAssigned { get; set; }
}