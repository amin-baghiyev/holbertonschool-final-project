using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using PLDMS.DL.Repositories.Abstractions;
using System.Linq.Expressions;

namespace PLDMS.BL.Services.Concretes;

public class ExerciseService : IExerciseService
{
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IRepository<Program> _programRepository;
    private readonly IMapper _mapper;

    public ExerciseService(IRepository<Exercise> exerciseRepository, IRepository<Program> programRepository, IMapper mapper)
    {
        _exerciseRepository = exerciseRepository;
        _programRepository = programRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ExerciseTableItemDTO>, int TotalCount)> ExercisesAsTableItemAsync(
        string? q = null,
        ICollection<int>? programs = null,
        ICollection<ProgrammingLanguage>? languages = null,
        ExerciseDifficulty? difficulty = null,
        bool onlyActive = true,
        int page = 0,
        int count = 25)
    {
        Expression<Func<Exercise, bool>> predicate = e =>
            (!onlyActive || !e.IsDeleted) &&
            (string.IsNullOrWhiteSpace(q) || e.Name.ToLower().Contains(q.ToLower())) &&
            (programs == null || programs.Count == 0 || programs.Contains(e.ProgramId)) &&
            (difficulty == null || e.Difficulty == difficulty) &&
            (languages == null || languages.Count == 0 || e.ExerciseLanguages.Any(el => languages.Contains(el.ProgrammingLanguage)));

        var (items, totalCount) = await _exerciseRepository.GetAllAsync(
            predicate: predicate,
            page: page,
            count: count,
            includes: query => query.Include(e => e.Program)
                                    .Include(e => e.ExerciseLanguages)
        );

        var dtos = _mapper.Map<IEnumerable<ExerciseTableItemDTO>>(items);
        return (dtos, totalCount);
    }

    public async Task<ICollection<ExerciseAsOptionDTO>> ExercisesAsOptionItemAsync(string? q = null, int count = 25)
    {
        Expression<Func<Exercise, bool>> predicate = e =>
            !e.IsDeleted &&
            (string.IsNullOrWhiteSpace(q) || e.Name.ToLower().Contains(q.ToLower()));

        var (items, _) = await _exerciseRepository.GetAllAsync(
            predicate: predicate,
            count: count
        );

        return _mapper.Map<ICollection<ExerciseAsOptionDTO>>(items);
    }

    public async Task<ExerciseDetailDTO> ExerciseByIdAsync(long id)
    {
        var exercise = await _exerciseRepository.GetOneAsync(
            predicate: e => e.Id == id,
            includes: query => query.Include(e => e.Program)
                                    .Include(e => e.TestCases)
                                    .Include(e => e.ExerciseLanguages)
        ) ?? throw new BaseException($"Exercise with ID {id} was not found");

        return _mapper.Map<ExerciseDetailDTO>(exercise);
    }

    public async Task<ExerciseFormDTO> ExerciseByIdForStudentAsync(long id)
    {
        var exercise = await _exerciseRepository.GetOneAsync(
            predicate: e => e.Id == id && !e.IsDeleted,
            includes: query => query.Include(e => e.TestCases)
                                    .Include(e => e.ExerciseLanguages)
        ) ?? throw new BaseException($"Exercise with ID {id} was not found");

        var dto = _mapper.Map<ExerciseFormDTO>(exercise);
        dto.TestCases = exercise.TestCases
            .Where(tc => tc.IsExample && !tc.IsDeleted)
            .Select(tc => _mapper.Map<ExerciseTestCasesDTO>(tc))
            .ToList();

        return dto;
    }

    public async Task<ExerciseFormDTO> ExerciseByIdForEditAsync(long id)
    {
        var exercise = await _exerciseRepository.GetOneAsync(
            predicate: e => e.Id == id,
            includes: query => query.Include(e => e.TestCases)
                                    .Include(e => e.ExerciseLanguages)
        ) ?? throw new BaseException($"Exercise with ID {id} was not found");

        return _mapper.Map<ExerciseFormDTO>(exercise);
    }

    public async Task CreateAsync(ExerciseFormDTO dto)
    {
        if (await _programRepository.GetOneAsync(p => p.Id == dto.ProgramId) is null)
            throw new BaseException($"Program with ID {dto.ProgramId} was not found");

        var newExercise = _mapper.Map<Exercise>(dto);

        await _exerciseRepository.CreateAsync(newExercise);
        await _exerciseRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(long id, ExerciseFormDTO dto)
    {
        if (await _programRepository.GetOneAsync(p => p.Id == dto.ProgramId) is null)
            throw new BaseException($"Program with ID {dto.ProgramId} was not found");

        var exercise = await _exerciseRepository.GetOneAsync(
            predicate: e => e.Id == id,
            includes: query => query.Include(e => e.TestCases)
                                    .Include(e => e.ExerciseLanguages),
            isTracking: true
        ) ?? throw new BaseException($"Exercise with ID {id} was not found");

        exercise.Name = dto.Name;
        exercise.Description = dto.Description;
        exercise.Difficulty = dto.Difficulty;
        exercise.ProgramId = dto.ProgramId;

        var incomingLanguages = dto.Languages.ToList();

        var languagesToRemove = exercise.ExerciseLanguages
            .Where(el => !incomingLanguages.Contains(el.ProgrammingLanguage)).ToList();
        foreach (var lang in languagesToRemove)
        {
            exercise.ExerciseLanguages.Remove(lang);
        }

        var existingLanguages = exercise.ExerciseLanguages.Select(el => el.ProgrammingLanguage).ToList();
        var languagesToAdd = incomingLanguages.Where(l => !existingLanguages.Contains(l)).ToList();
        foreach (var lang in languagesToAdd)
        {
            exercise.ExerciseLanguages.Add(new ExerciseLanguage { ProgrammingLanguage = lang });
        }

        var incomingTestCases = dto.TestCases.ToList();

        var testCasesToRemove = exercise.TestCases
            .Where(dbTc => !incomingTestCases.Any(dtoTc => dtoTc.Input == dbTc.Input && dtoTc.Output == dbTc.Output))
            .ToList();

        foreach (var tc in testCasesToRemove)
        {
            exercise.TestCases.Remove(tc);
        }

        // Update IsExample flag for existing test cases
        foreach (var dtoTc in incomingTestCases)
        {
            var dbTc = exercise.TestCases.FirstOrDefault(t => t.Input == dtoTc.Input && t.Output == dtoTc.Output);
            if (dbTc != null)
            {
                dbTc.IsExample = dtoTc.IsExample;
            }
        }

        var testCasesToAdd = incomingTestCases
            .Where(dtoTc => !exercise.TestCases.Any(dbTc => dbTc.Input == dtoTc.Input && dbTc.Output == dtoTc.Output))
            .ToList();
        foreach (var tc in testCasesToAdd)
        {
            exercise.TestCases.Add(new TestCase { Input = tc.Input, Output = tc.Output, IsExample = tc.IsExample });
        }

        _exerciseRepository.Update(exercise);
        await _exerciseRepository.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(long id)
    {
        var exercise = await _exerciseRepository.GetOneAsync(e => e.Id == id, isTracking: true) ?? throw new BaseException($"Exercise with ID {id} was not found");
        exercise.IsDeleted = true;

        _exerciseRepository.Update(exercise);
        await _exerciseRepository.SaveChangesAsync();
    }

    public async Task RecoverAsync(long id)
    {
        var exercise = await _exerciseRepository.GetOneAsync(e => e.Id == id, isTracking: true) ?? throw new BaseException($"Exercise with ID {id} was not found");
        exercise.IsDeleted = false;

        _exerciseRepository.Update(exercise);
        await _exerciseRepository.SaveChangesAsync();
    }
}