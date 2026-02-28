using PLDMS.BL.DTOs.ProgramDTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface IProgramService
{
    Task<ICollection<ProgramItemDTO>> ProgramsAsItemAsync(string q);
    Task CreateAsync(ProgramFormDTO dto);
    Task UpdateAsync(int id, ProgramFormDTO dto);
    Task DeleteAsync(int id);
    Task SoftDeleteAsync(int id);
    Task RevertSoftDeleteAsync(int id);
    Task<int> SaveChangesAsync();
}