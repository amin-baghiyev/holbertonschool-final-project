using PLDMS.BL.DTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface ISubmissionService
{
    Task<CodeSubmissionResultDTO> SubmitCodeAsync(Guid studentId, CodeSubmissionDTO dto);
    Task<CodeSubmissionResultDTO> RunCodeAsync(Guid studentId, CodeSubmissionDTO dto);
    Task<IEnumerable<SubmissionListItemDTO>> GetSubmissionsByGroupAsync(Guid groupId, long exerciseId);
    Task<string?> GetLastSubmittedCodeAsync(Guid groupId, long exerciseId);
}
