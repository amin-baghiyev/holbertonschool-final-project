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

public class SessionService : ISessionService
{
    private readonly IRepository<Session> _sessionRepository;
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IRepository<StudentCohort> _studentCohortRepository;
    private readonly IMapper _mapper;

    public SessionService(IRepository<Session> sessionRepository, IRepository<Exercise> exerciseRepository, IRepository<StudentCohort> studentCohortRepository, IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _exerciseRepository = exerciseRepository;
        _studentCohortRepository = studentCohortRepository;
        _mapper = mapper;
    }

    public async Task<(ICollection<SessionTableItemDTO>, int TotalCount)> SessionsAsTableItemAsync(
        string? q = null, DateTime? startDate = null, DateTime? endDate = null, SessionStatus? status = null, int page = 0, int count = 25)
    {
        var now = DateTime.UtcNow;

        Expression<Func<Session, bool>> predicate = s =>
            (string.IsNullOrWhiteSpace(q) || s.Name.Contains(q)) &&
            s.StartDate >= startDate &&
            s.EndDate <= endDate &&
            (
                status == SessionStatus.Upcoming ? s.StartDate > now :
                status == SessionStatus.Finished ? s.EndDate < now :
                (s.StartDate <= now && s.EndDate >= now)
            );

        var (sessions, totalCount) = await _sessionRepository.GetAllAsync(
            predicate: predicate,
            page: page,
            count: count,
            includes: query => query.Include(s => s.Cohort).ThenInclude(c => c.Program),
            orderAsc: false,
            orderBy: "CreatedAt",
            isTracking: false
        );

        return (_mapper.Map<ICollection<SessionTableItemDTO>>(sessions), totalCount);
    }

    public async Task<SessionDetailDTO> SessionByIdAsync(Guid id)
    {
        var session = await _sessionRepository.GetOneAsync(
            predicate: s => s.Id == id,
            includes: query => query
                .Include(s => s.Cohort).ThenInclude(c => c.Program!)
                .Include(s => s.Groups).ThenInclude(g => g.Students)
                .Include(s => s.Exercises).ThenInclude(se => se.Exercise).ThenInclude(e => e.ExerciseLanguages),
            isTracking: false
        );

        if (session == null)
            throw new BaseException($"Session with ID {id} was not found");

        return _mapper.Map<SessionDetailDTO>(session);
    }

    public async Task<SessionFormDTO> SessionByIdForEditAsync(Guid id)
    {
        var session = await _sessionRepository.GetOneAsync(
            predicate: s => s.Id == id,
            includes: query => query.Include(s => s.Exercises),
            isTracking: false
        );

        if (session == null)
            throw new BaseException($"Session with ID {id} was not found");

        var dto = _mapper.Map<SessionFormDTO>(session);
        dto.ExercisesIds = [..session.Exercises.Select(e => e.ExerciseId)];

        return dto;
    }

    public async Task CreateAsync(SessionFormDTO dto)
    {
        if (dto.StartDate.Date != dto.EndDate.Date)
            throw new BaseException("A session must start and end on the same day");

        bool hasOverlap = await _sessionRepository.Table.AnyAsync(s =>
            s.CohortId == dto.CohortId &&
            s.StartDate < dto.EndDate &&
            s.EndDate > dto.StartDate);

        if (hasOverlap)
            throw new BaseException("A session for this cohort already exists during this time period");

        var cohortStudentIds = await _studentCohortRepository.Table
            .Where(sc => sc.CohortId == dto.CohortId && !sc.IsDeleted)
            .Select(sc => sc.StudentId)
            .ToListAsync();

        if (cohortStudentIds.Count == 0)
            throw new BaseException("Cannot create a session for a cohort with no active students");

        bool studentOverlapExists = await _sessionRepository.Table
            .Where(s => s.StartDate < dto.EndDate && s.EndDate > dto.StartDate)
            .SelectMany(s => s.Groups)
            .SelectMany(g => g.Students)
            .AnyAsync(sg => cohortStudentIds.Contains(sg.StudentId));

        if (studentOverlapExists)
            throw new BaseException("One or more students in this cohort are already assigned to another session during this time frame");

        var existingExercisesCount = await _exerciseRepository.Table
            .CountAsync(e => dto.ExercisesIds.Contains(e.Id));

        if (existingExercisesCount != dto.ExercisesIds.Count)
            throw new BaseException("One or more selected exercises do not exist in the database");

        var session = _mapper.Map<Session>(dto);
        session.Id = Guid.CreateVersion7();
        session.CreatedAt = DateTime.UtcNow;
        session.RepositoryUrl = string.Empty;

        session.Name = string.IsNullOrWhiteSpace(dto.Name)
            ? $"Session-{Guid.NewGuid().ToString()[..8]}"
            : dto.Name;

        session.Exercises = [.. dto.ExercisesIds.Select(id => new SessionExercise
        {
            ExerciseId = id
        })];

        var random = new Random();
        var shuffledStudentIds = cohortStudentIds.OrderBy(x => random.Next()).ToList();

        var groups = new List<Group>();
        for (int i = 0; i < shuffledStudentIds.Count; i += dto.StudentCountPerGroup)
        {
            var groupStudents = shuffledStudentIds.Skip(i).Take(dto.StudentCountPerGroup).ToList();

            groups.Add(new Group
            {
                Name = $"Group-{Guid.NewGuid().ToString()[..8]}",
                TotalStudentCount = groupStudents.Count,
                Students = [..groupStudents.Select(id => new StudentGroup { StudentId = id })]
            });
        }

        session.Groups = groups;
        session.TotalStudentCount = cohortStudentIds.Count;

        await _sessionRepository.CreateAsync(session);
        await _sessionRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, SessionFormDTO dto)
    {
        var session = await _sessionRepository.GetOneAsync(
            predicate: s => s.Id == id,
            includes: query => query.Include(s => s.Exercises),
            isTracking: true
        );

        if (session == null)
            throw new BaseException($"Session with ID {id} was not found");

        if (dto.StartDate.Date != dto.EndDate.Date)
            throw new BaseException("A session must start and end on the same day");

        bool hasOverlap = await _sessionRepository.Table.AnyAsync(s =>
            s.Id != id &&
            s.CohortId == dto.CohortId &&
            s.StartDate < dto.EndDate &&
            s.EndDate > dto.StartDate);

        if (hasOverlap)
            throw new BaseException("A session for this cohort already exists during this time period");

        bool hasStarted = session.StartDate <= DateTime.UtcNow;

        bool cohortChanged = session.CohortId != dto.CohortId;
        bool groupSizeChanged = session.StudentCountPerGroup != dto.StudentCountPerGroup;
        bool datesChanged = session.StartDate != dto.StartDate || session.EndDate != dto.EndDate;

        if (cohortChanged || groupSizeChanged || datesChanged)
        {
            if (hasStarted)
                throw new BaseException("Cannot change cohort, group sizes, or dates because the session has already started");

            bool hasCohortOverlap = await _sessionRepository.Table.AnyAsync(s =>
                s.Id != id &&
                s.CohortId == dto.CohortId &&
                s.StartDate < dto.EndDate &&
                s.EndDate > dto.StartDate);

            if (hasCohortOverlap)
                throw new BaseException("A session for this cohort already exists during this time period");

            if (cohortChanged || groupSizeChanged)
            {
                var cohortStudentIds = await _studentCohortRepository.Table
                    .Where(sc => sc.CohortId == dto.CohortId && !sc.IsDeleted)
                    .Select(sc => sc.StudentId)
                    .ToListAsync();

                if (cohortStudentIds.Count == 0)
                    throw new BaseException("Cannot update to a cohort with no active students");

                bool studentOverlapExists = await _sessionRepository.Table
                    .Where(s => s.Id != id && s.StartDate < dto.EndDate && s.EndDate > dto.StartDate)
                    .SelectMany(s => s.Groups)
                    .SelectMany(g => g.Students)
                    .AnyAsync(sg => cohortStudentIds.Contains(sg.StudentId));

                if (studentOverlapExists)
                    throw new BaseException("One or more students in this cohort are already assigned to another session during this time frame");

                session.Groups.Clear();

                var random = new Random();
                var shuffledStudentIds = cohortStudentIds.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < shuffledStudentIds.Count; i += dto.StudentCountPerGroup)
                {
                    var groupIds = shuffledStudentIds.Skip(i).Take(dto.StudentCountPerGroup).ToList();
                    session.Groups.Add(new Group
                    {
                        Name = $"Group-{Guid.NewGuid().ToString()[..8]}",
                        TotalStudentCount = groupIds.Count,
                        Students = groupIds.Select(sid => new StudentGroup { StudentId = sid }).ToList()
                    });
                }

                session.TotalStudentCount = cohortStudentIds.Count;
                session.CohortId = (int)dto.CohortId!;
                session.StudentCountPerGroup = dto.StudentCountPerGroup;
            }
        }

        var currentExerciseIds = session.Exercises.Select(e => e.ExerciseId).ToList();
        var exercisesChanged = !currentExerciseIds.All(dto.ExercisesIds.Contains) || currentExerciseIds.Count != dto.ExercisesIds.Count;

        if (hasStarted && exercisesChanged)
            throw new BaseException("Cannot modify tasks because the session has already started");

        if (exercisesChanged)
        {
            if (hasStarted)
                throw new BaseException("Cannot modify tasks because the session has already started");

            var existingCount = await _exerciseRepository.Table.CountAsync(e => dto.ExercisesIds.Contains(e.Id));
            if (existingCount != dto.ExercisesIds.Count)
                throw new BaseException("One or more selected exercises do not exist");

            session.Exercises.Clear();
            foreach (var exId in dto.ExercisesIds)
            {
                session.Exercises.Add(new SessionExercise
                {
                    SessionId = session.Id,
                    ExerciseId = exId
                });
            }
        }

        session.Name = string.IsNullOrWhiteSpace(dto.Name) ? session.Name : dto.Name;
        session.StartDate = dto.StartDate;
        session.EndDate = dto.EndDate;

        _sessionRepository.Update(session);
        await _sessionRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await _sessionRepository.GetOneAsync(s => s.Id == id, isTracking: true);

        if (session == null)
            throw new BaseException($"Session with ID {id} was not found");

        if (session.StartDate <= DateTime.UtcNow)
            throw new BaseException("Cannot delete the session because it has already started");

        _sessionRepository.Delete(session);
        await _sessionRepository.SaveChangesAsync();
    }
}