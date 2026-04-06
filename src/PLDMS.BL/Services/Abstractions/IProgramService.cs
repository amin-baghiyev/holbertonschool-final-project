using PLDMS.BL.DTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface IProgramService
{
    Task<ICollection<ProgramItemDTO>> ProgramsAsItemAsync(string q, bool onlyActive = true);
    Task<ICollection<ProgramOptionItemDTO>> ProgramsAsOptionItemAsync(string? q = null, int count = 25);
    Task CreateAsync(ProgramFormDTO dto);
    Task UpdateAsync(int id, ProgramFormDTO dto);
    Task SoftDeleteAsync(int id);
    Task RevertSoftDeleteAsync(int id);
    Task<int> SaveChangesAsync();
}