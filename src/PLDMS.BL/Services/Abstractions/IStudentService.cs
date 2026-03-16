using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Abstractions;

public interface IStudentService
{
    Task<(ICollection<StudentSessionListItemDTO> Items, int TotalCount)> GetSessionsAsync(
        Guid studentId, string? q = null, DateTime? startDate = null, DateTime? endDate = null, SessionStatus? status = null, int page = 0, int count = 25);

    Task<(ICollection<StudentReviewListItemDTO> Items, int TotalCount)> GetReviewsAsync(
        Guid studentId, ReviewStatus? status = null, int page = 0, int count = 25);

    Task<StudentReviewCreateDTO?> GetReviewForEditAsync(Guid reviewId, Guid studentId);
    
    Task SubmitReviewAsync(Guid reviewId, Guid studentId, StudentReviewCreateDTO dto);

    Task<(ICollection<StudentTableItemDTO> Items, int TotalCount)> StudentsAsTableItemAsync(string q, int page, int count);
    Task CreateAsync(StudentFormDTO dto);
    Task UpdateAsync(Guid id, StudentFormDTO dto);
    Task DeleteAsync(Guid id);
    Task<int> SaveChangesAsync();
}
