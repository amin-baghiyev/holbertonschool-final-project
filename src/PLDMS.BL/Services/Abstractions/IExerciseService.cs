using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Abstractions;

public interface IExerciseService
{
    Task<(IEnumerable<ExerciseTableItemDTO>, int TotalCount)> ExercisesAsTableItemAsync(string? q = null, ICollection<int>? programs = null, ICollection<ProgrammingLanguage>? languages = null, ExerciseDifficulty? difficulty = null, bool onlyActive = true, int page = 0, int count = 25);
    Task<ICollection<ExerciseAsOptionDTO>> ExercisesAsOptionItemAsync(string? q = null, int count = 25);
    Task<ExerciseDetailDTO> ExerciseByIdAsync(long id);
    Task<ExerciseFormDTO> ExerciseByIdForEditAsync(long id);
    Task<ExerciseFormDTO> ExerciseByIdForStudentAsync(long id);
    Task CreateAsync(ExerciseFormDTO dto);
    Task UpdateAsync(long id, ExerciseFormDTO dto);
    Task SoftDeleteAsync(long id);
    Task RecoverAsync(long id);
}