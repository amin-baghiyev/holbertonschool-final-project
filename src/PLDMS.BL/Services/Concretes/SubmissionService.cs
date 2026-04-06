using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.DL.Repositories.Abstractions;

namespace PLDMS.BL.Services.Concretes;

public class SubmissionService : ISubmissionService
{
    private readonly IRepository<Submission> _submissionRepository;
    private readonly IRepository<Group> _groupRepository;
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IJudgeService _judgeService;
    private readonly PLDMS.BL.Utilities.GitHubService _gitHubService;

    public SubmissionService(
        IRepository<Submission> submissionRepository,
        IRepository<Group> groupRepository,
        IRepository<Exercise> exerciseRepository,
        IJudgeService judgeService,
        PLDMS.BL.Utilities.GitHubService gitHubService)
    {
        _submissionRepository = submissionRepository;
        _groupRepository = groupRepository;
        _exerciseRepository = exerciseRepository;
        _judgeService = judgeService;
        _gitHubService = gitHubService;
    }

    public async Task<CodeSubmissionResultDTO> RunCodeAsync(Guid studentId, CodeSubmissionDTO dto)
    {
        var (group, exercise, testCases) = await ValidateSubmissionAsync(studentId, dto);
        var exampleTestCases = testCases.Where(tc => tc.IsExample).ToList();
        return await ExecuteAndEvaluateCodeAsync(dto, exampleTestCases);
    }

    public async Task<CodeSubmissionResultDTO> SubmitCodeAsync(Guid studentId, CodeSubmissionDTO dto)
    {
        var (group, exercise, testCases) = await ValidateSubmissionAsync(studentId, dto);
        
        var submissionResult = await ExecuteAndEvaluateCodeAsync(dto, testCases);
        var testsPassedArray = submissionResult.TestResults.Select(tr => tr.Passed).ToArray();
        string commitHash = "N/A";

        var langExt = dto.LanguageId switch
        {
            PLDMS.Core.Enums.ProgrammingLanguage.C => ".c",
            PLDMS.Core.Enums.ProgrammingLanguage.JavaScript => ".js",
            PLDMS.Core.Enums.ProgrammingLanguage.Java => ".java",
            PLDMS.Core.Enums.ProgrammingLanguage.Python => ".py",
            _ => ".txt"
        };
        
        var fileName = $"{exercise.Name}{langExt}";
        var commitMessage = testsPassedArray.All(t => t) 
            ? $"feat: completed {exercise.Name}" 
            : $"fix: attempt {exercise.Name} ({testsPassedArray.Count(t => t)}/{testsPassedArray.Length} passed)";
        
        try 
        {
            commitHash = await _gitHubService.CommitCodeAsync(
                group.Session.RepositoryUrl, 
                group.Name, 
                fileName, 
                dto.SourceCode, 
                commitMessage);
        }
        catch (Exception ex)
        {
            commitHash = $"Error: {ex.Message}";
        }

        var submission = new Submission
        {
            Id = Guid.CreateVersion7(),
            GroupId = dto.GroupId,
            ExerciseId = dto.ExerciseId,
            CommitHash = commitHash,
            ProgrammingLanguage = dto.LanguageId,
            Tests = testsPassedArray,
            CreatedAt = DateTime.UtcNow.AddHours(4)
        };

        await _submissionRepository.CreateAsync(submission);
        await _submissionRepository.SaveChangesAsync();

        return submissionResult;
    }

    public async Task<IEnumerable<SubmissionListItemDTO>> GetSubmissionsByGroupAsync(Guid groupId, long exerciseId)
    {
        var submissions = await _submissionRepository.Table
            .Where(s => s.GroupId == groupId && s.ExerciseId == exerciseId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return submissions.Select(s => new SubmissionListItemDTO
        {
            Id = s.Id,
            CommitHash = s.CommitHash,
            ProgrammingLanguage = s.ProgrammingLanguage,
            PassCount = s.Tests.Count(t => t),
            TotalTests = s.Tests.Length,
            CreatedAt = s.CreatedAt
        });
    }

    public async Task<string?> GetLastSubmittedCodeAsync(Guid groupId, long exerciseId)
    {
        var lastSubmission = await _submissionRepository.Table
            .Include(s => s.Group).ThenInclude(g => g.Session)
            .Include(s => s.Exercise)
            .Where(s => s.GroupId == groupId && s.ExerciseId == exerciseId && !s.CommitHash.StartsWith("Error:") && s.CommitHash != "N/A")
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastSubmission == null)
            return null;

        var langExt = lastSubmission.ProgrammingLanguage switch
        {
            PLDMS.Core.Enums.ProgrammingLanguage.C => ".c",
            PLDMS.Core.Enums.ProgrammingLanguage.JavaScript => ".js",
            PLDMS.Core.Enums.ProgrammingLanguage.Java => ".java",
            PLDMS.Core.Enums.ProgrammingLanguage.Python => ".py",
            _ => ".txt"
        };

        var fileName = $"{lastSubmission.Exercise.Name}{langExt}";

        try
        {
            return await _gitHubService.GetFileContentAsync(
                lastSubmission.Group.Session.RepositoryUrl,
                lastSubmission.Group.Name,
                fileName,
                lastSubmission.CommitHash);
        }
        catch
        {
            return null;
        }
    }

    private async Task<(Group group, Exercise exercise, List<TestCase> testCases)> ValidateSubmissionAsync(Guid studentId, CodeSubmissionDTO dto)
    {
        var group = await _groupRepository.GetOneAsync(
            predicate: g => g.Id == dto.GroupId && g.Students.Any(s => s.StudentId == studentId),
            includes: query => query.Include(g => g.Session),
            isTracking: false);

        if (group == null)
            throw new BaseException("Group not found or you do not have permission.");

        if (group.Session.EndDate < DateTime.UtcNow.AddHours(4))
            throw new BaseException("Cannot submit code because the session has ended.");

        var exercise = await _exerciseRepository.GetOneAsync(
            predicate: e => e.Id == dto.ExerciseId && !e.IsDeleted,
            includes: query => query.Include(e => e.TestCases).Include(e => e.ExerciseLanguages),
            isTracking: false);

        if (exercise == null)
            throw new BaseException("Exercise not found.");

        if (!exercise.ExerciseLanguages.Any(el => el.ProgrammingLanguage == dto.LanguageId))
            throw new BaseException("Selected language is not supported for this exercise.");

        var testCases = exercise.TestCases.Where(tc => !tc.IsDeleted).ToList();
        
        if (testCases.Count == 0)
            throw new BaseException("This exercise has no test cases configured.");

        return (group, exercise, testCases);
    }

    private async Task<CodeSubmissionResultDTO> ExecuteAndEvaluateCodeAsync(CodeSubmissionDTO dto, List<TestCase> testCases)
    {
        var submissionResult = new CodeSubmissionResultDTO
        {
            TotalTests = testCases.Count,
            PassedTests = 0
        };

        for (int i = 0; i < testCases.Count; i++)
        {
            var tc = testCases[i];

            try
            {
                var judgeResponse = await _judgeService.ExecuteCodeAsync(
                    dto.LanguageId,
                    dto.SourceCode,
                    tc.Input
                );

                var actualOutput = NormalizeOutput(judgeResponse.stdout);
                var expectedOutput = NormalizeOutput(tc.Output);

                var tcResult = new TestCaseResultDTO
                {
                    TestCaseIndex = i + 1,
                    IsExample = tc.IsExample,
                    ExpectedOutput = tc.IsExample ? tc.Output : null,
                    ActualOutput = tc.IsExample ? judgeResponse.stdout : null
                };

                if (judgeResponse.status?.id == 3 && actualOutput == expectedOutput)
                {
                    tcResult.Passed = true;
                    submissionResult.PassedTests++;
                }
                else
                {
                    tcResult.Passed = false;

                    tcResult.ErrorMessage =
                        judgeResponse.compile_output ??
                        judgeResponse.stderr ??
                        judgeResponse.status?.description ??
                        "Wrong Answer";
                }

                submissionResult.TestResults.Add(tcResult);
            }
            catch (Exception ex)
            {
                submissionResult.TestResults.Add(new TestCaseResultDTO
                {
                    TestCaseIndex = i + 1,
                    Passed = false,
                    IsExample = tc.IsExample,
                    ErrorMessage = tc.IsExample ? $"Execution failed: {ex.Message}" : "Execution Error"
                });
            }
        }

        submissionResult.Success = submissionResult.TotalTests == submissionResult.PassedTests;
        submissionResult.Message = submissionResult.Success
            ? "All tests passed!"
            : "Some tests failed. Check the results.";

        return submissionResult;
    }

    private static string NormalizeOutput(string? output)
    {
        if (string.IsNullOrWhiteSpace(output))
            return string.Empty;

        return output
            .Trim()
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");
    }
}