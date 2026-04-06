using PLDMS.BL.DTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface ICohortService
{
    Task<ICollection<StudentSelectItemDTO>> GetStudentSelectItemsByCohortIdAsync(int id);
    Task SyncStudentsInCohortAsync(int cohortId, IEnumerable<Guid> studentIds);
    Task<ICollection<StudentSelectItemDTO>> GetStudentSelectItemsAsync(string? q = null, int count = 25);
    Task<(ICollection<CohortTableItemDTO> Items, int TotalCount)> CohortsAsTableItemAsync(string q, bool onlyActive = true, int page = 0, int count = 10);
    Task<ICollection<CohortOptionItemDTO>> CohortsAsOptionItemAsync(string? q = null, int count = 25);
    Task<ICollection<ProgramOptionItemDTO>> GetProgramSelectItemsAsync();
    Task CreateAsync(CohortFormDTO dto);
    Task UpdateAsync(int id, CohortFormDTO dto);
    Task SoftDeleteAsync(int id);
    Task RevertSoftDeleteAsync(int id);
    Task<int> SaveChangesAsync();
}