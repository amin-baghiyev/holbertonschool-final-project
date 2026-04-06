using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Abstractions;

public interface IJudgeService
{
    Task<Judge0ResponseDTO> ExecuteCodeAsync(ProgrammingLanguage language, string sourceCode, string? stdin = null);
}
