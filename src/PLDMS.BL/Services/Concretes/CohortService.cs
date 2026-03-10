using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.DL.Repositories.Abstractions;

namespace PLDMS.BL.Services.Concretes;

public class CohortService : ICohortService
{
    private readonly IRepository<Cohort> _cohortRepository;
    private readonly IRepository<Program> _programRepository;
    private readonly IMapper _mapper;
    
    public CohortService(IRepository<Cohort> cohortRepository, IRepository<Program> programRepository, IMapper mapper)
    {
        _cohortRepository = cohortRepository;
        _programRepository = programRepository;
        _mapper = mapper;
    }
    
    public async Task<(ICollection<CohortTableItemDTO> Items, int TotalCount)> CohortsAsTableItemAsync(string q, int page = 0, int count = 10)
    {
        var (cohorts, totalCount) = await _cohortRepository.GetAllAsync(c=> 
            string.IsNullOrWhiteSpace(q) ||
            EF.Functions.ILike(c.Name, $"%{q}%") ||
            EF.Functions.ILike(c.Program.Name, $"%{q}%"),
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

    public async Task DeleteAsync(int id)
    {
        var cohort = await _cohortRepository.GetOneAsync(c => c.Id == id);

        if (cohort == null)
        {
            throw new BaseException("Cohort not found");
        }
        
        _cohortRepository.Delete(cohort);
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