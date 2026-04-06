using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using PLDMS.DL.Contexts;
using PLDMS.DL.Repositories.Abstractions;

namespace PLDMS.BL.Services.Concretes;

public class CohortService : ICohortService
{
    private readonly AppDbContext _context;
    private readonly IRepository<Cohort> _cohortRepository;
    private readonly IRepository<Program> _programRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    
    public CohortService(AppDbContext context, IRepository<Cohort> cohortRepository,
        IRepository<Program> programRepository, UserManager<AppUser> userManager, IMapper mapper)
    {
        _context = context;
        _cohortRepository = cohortRepository;
        _programRepository = programRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task SyncStudentsInCohortAsync(int cohortId, IEnumerable<Guid> studentIds)
    {
        var cohort = await _context.Cohorts.FirstOrDefaultAsync(c => c.Id == cohortId);
        if (cohort == null) throw new BaseException("Cohort not found.");

        var currentAssignments = await _context.StudentCohorts
            .Where(sc => sc.CohortId == cohortId)
            .ToListAsync();

        var currentStudentIds = currentAssignments.Select(sc => sc.StudentId).ToList();

        var assignmentsToRemove = currentAssignments
            .Where(sc => !studentIds.Contains(sc.StudentId))
            .ToList();

        var studentIdsToAdd = studentIds
            .Except(currentStudentIds)
            .Select(sId => new StudentCohort
            {
                CohortId = cohortId,
                StudentId = sId
            })
            .ToList();

        if (assignmentsToRemove.Any())
        {
            _context.StudentCohorts.RemoveRange(assignmentsToRemove);
        }

        if (studentIdsToAdd.Any())
        {
            await _context.StudentCohorts.AddRangeAsync(studentIdsToAdd);
        }
        
        cohort.TotalStudentCount = studentIds.Count();
    }

    public async Task<ICollection<StudentSelectItemDTO>> GetStudentSelectItemsByCohortIdAsync(int id)
    {
        return await _context.StudentCohorts
            .AsNoTracking()
            .Where(sc => sc.CohortId == id)
            .Select(sc => new StudentSelectItemDTO
            {
                Id = sc.Student.Id,
                FullName = sc.Student.FullName,
                Email = sc.Student.Email,
            })
            .ToListAsync();
    }
    
    public async Task<ICollection<StudentSelectItemDTO>> GetStudentSelectItemsAsync(string? q = null, int count = 25)
    {
        var students = _userManager.Users.Where(u => u.Role == UserRole.Student);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var qPattern = $"%{q}%";
            students = students.Where(u => EF.Functions.ILike(u.FullName, qPattern) || EF.Functions.ILike(u.Email, qPattern));
        }

        var items = await students
            .Take(count)
            .Select(u => new StudentSelectItemDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            }).ToListAsync();

        return items;
    }

    public async Task<(ICollection<CohortTableItemDTO> Items, int TotalCount)> CohortsAsTableItemAsync(string q, bool onlyActive = true, int page = 0, int count = 10)
    {
        var (cohorts, totalCount) = await _cohortRepository.GetAllAsync(c=> 
            (!onlyActive || !c.IsDeleted) &&
            (string.IsNullOrWhiteSpace(q) ||
            EF.Functions.ILike(c.Name, $"%{q}%") ||
            EF.Functions.ILike(c.Program.Name, $"%{q}%")),
            page,
            count,
            includes: cohort => cohort.Include(c => c.Program)
        );

        ICollection<CohortTableItemDTO> dto = cohorts.Select(c => new CohortTableItemDTO
        {
            Id = c.Id,
            Name = c.Name,
            Program = new ProgramOptionItemDTO
            {
                Id = c.Program.Id,
                Name = c.Program.Name
            },
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            StudentCount = c.TotalStudentCount,
            IsDeleted = c.IsDeleted,
        }).ToList();

        return (dto, totalCount);
    }

    public async Task<ICollection<CohortOptionItemDTO>> CohortsAsOptionItemAsync(string? q = null, int count = 25)
    {
        var (cohorts, _) = await _cohortRepository.GetAllAsync(c =>
            !c.IsDeleted &&
            (string.IsNullOrWhiteSpace(q) || EF.Functions.ILike(c.Name, $"%{q}%")),
            count
        );

        return _mapper.Map<ICollection<CohortOptionItemDTO>>(cohorts);
    }
    
    public async Task<ICollection<ProgramOptionItemDTO>> GetProgramSelectItemsAsync()
    {
        var (programs, totalCount) = await _programRepository.GetAllAsync();

        ICollection<ProgramOptionItemDTO> selectItemDtos = programs.Select(p => new ProgramOptionItemDTO
        {
            Id = p.Id,
            Name = p.Name,
        }).ToList();
        
        return selectItemDtos;
    }

    public async Task CreateAsync(CohortFormDTO dto)
    {
        if (await _programRepository.GetOneAsync(p => p.Id == dto.ProgramId) is null)
            throw new BaseException($"Program with ID {dto.ProgramId} was not found");

        var cohort = _mapper.Map<Cohort>(dto);
        await _cohortRepository.CreateAsync(cohort);
    }

    public async Task UpdateAsync(int id, CohortFormDTO dto)
    {
        if (await _programRepository.GetOneAsync(p => p.Id == dto.ProgramId) is null)
            throw new BaseException($"Program with ID {dto.ProgramId} was not found");

        var cohort = await _cohortRepository.GetOneAsync(c => c.Id == id);

        if (cohort == null)
        {
            throw new BaseException("Cohort not found");
        }
        
        if (cohort.IsDeleted)
        {
            throw new BaseException("Cohort is not active.");
        }
        
        cohort.Name = dto.Name;
        cohort.ProgramId = dto.ProgramId;
        cohort.StartDate = dto.StartDate;
        cohort.EndDate = dto.EndDate;
        
        _cohortRepository.Update(cohort);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var cohort = await _cohortRepository.GetOneAsync(c => c.Id == id);

        if (cohort == null)
        {
            throw new BaseException("Cohort not found");
        }
        
        if (cohort.IsDeleted)
        {
            throw new BaseException("Cohort is already deleted.");
        }
        
        cohort.IsDeleted = true;
        _cohortRepository.Update(cohort);
    }

    public async Task RevertSoftDeleteAsync(int id)
    {
        var cohort = await _cohortRepository.GetOneAsync(c => c.Id == id);

        if (cohort == null)
        {
            throw new BaseException("Cohort not found");
        }
        
        if (!cohort.IsDeleted)
        {
            throw new BaseException("Cohort is already reverted.");
        }
        
        cohort.IsDeleted = false;
        _cohortRepository.Update(cohort);
    }

    public Task<int> SaveChangesAsync()
    {
        return _cohortRepository.SaveChangesAsync();
    }
}