using PLDMS.BL.DTOs.MentorDTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface IMentorService
{
    Task<(ICollection<MentorTableItemDTO> Items, int TotalCount)> MentorsAsTableItemAsync(string q, int page = 0, int count = 10);
    Task CreateAsync(MentorFormDTO dto);
    Task UpdateAsync(Guid id, MentorFormDTO dto);
    Task DeleteAsync(Guid id);
    Task<int> SaveChangesAsync();
}