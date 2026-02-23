using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Abstractions;

public interface ISessionService
{
    Task<SessionTableItemDTO> SessionsAsTableItemAsync(string q, DateTime startDate, DateTime endDate, SessionStatus status, int page = 0, int count = 25);
    Task<SessionDetailDTO> SessionByIdAsync(Guid Id);
    Task CreateAsync(SessionFormDTO dto);
    Task UpdateAsync(Guid id, SessionFormDTO dto);
    Task DeleteAsync(Guid id);
}