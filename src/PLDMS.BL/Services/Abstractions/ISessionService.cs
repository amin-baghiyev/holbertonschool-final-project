using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Abstractions;

public interface ISessionService
{
    Task<(ICollection<SessionTableItemDTO>, int TotalCount)> SessionsAsTableItemAsync(string? q = null, int? cohortId = null, int? programId = null, DateTime? startDate = null, DateTime? endDate = null, SessionStatus? status = null, int page = 0, int count = 25);
    Task<SessionDetailDTO> SessionByIdAsync(Guid Id);
    Task<SessionFormDTO> SessionByIdForEditAsync(Guid Id);
    Task CreateAsync(SessionFormDTO dto);
    Task UpdateAsync(Guid id, SessionFormDTO dto);
    Task DeleteAsync(Guid id);
    Task<ICollection<ExerciseTableItemDTO>> GetExercisesBySessionIdAsync(Guid id);
}