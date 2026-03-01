using PLDMS.BL.DTOs.CohortDTOs;
using PLDMS.BL.DTOs.ProgramDTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface ICohortService
{
    Task<(ICollection<CohortTableItemDTO> Items, int TotalCount)> CohortsAsTableItemAsync(string q, int page = 0, int count = 10);
    Task<ICollection<ProgramSelectItemDTO>> GetProgramSelectItemsAsync();
    Task CreateAsync(CohortFormDTO dto);
    Task UpdateAsync(int id, CohortFormDTO dto);
    Task DeleteAsync(int id);
    Task SoftDeleteAsync(int id);
    Task RevertSoftDeleteAsync(int id);
    Task<int> SaveChangesAsync();
}