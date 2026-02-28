using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Concretes;

public class SessionService : ISessionService
{
    public Task<SessionTableItemDTO> SessionsAsTableItemAsync(string q, DateTime startDate, DateTime endDate, SessionStatus status, int page = 0,
        int count = 25)
    {
        throw new NotImplementedException();
    }

    public Task<SessionDetailDTO> SessionByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(SessionFormDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, SessionFormDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}