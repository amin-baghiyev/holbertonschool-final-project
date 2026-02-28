using AutoMapper;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs.ProgramDTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.DL.Repositories.Abstractions;

namespace PLDMS.BL.Services.Concretes;

public class ProgramService : IProgramService
{
    private readonly IRepository<Program> _programRepository;
    private readonly IMapper _mapper;
    
    public ProgramService(IRepository<Program> programRepository, IMapper mapper)
    {
        _programRepository = programRepository;
        _mapper = mapper;
    }
    
    public async Task<ICollection<ProgramItemDTO>> ProgramsAsItemAsync(string q)
    {
        return _mapper.Map<ICollection<ProgramItemDTO>>(
            await _programRepository.GetAllAsync(p =>
                string.IsNullOrEmpty(q) || p.Name.Contains(q)));
    }

    public async Task CreateAsync(ProgramFormDTO dto)
    {
        var program = _mapper.Map<Program>(dto);
        await _programRepository.CreateAsync(program);
    }

    public async Task UpdateAsync(int id, ProgramFormDTO dto)
    {
        var program = await _programRepository.GetOneAsync(p => p.Id == id);

        if (program == null)
        {
            throw new BaseException("Program not found");
        }
        
        if (program.IsDeleted)
        {
            throw new BaseException("Program is not active.");
        }
        
        program.Name = dto.Name;
        program.Duration = dto.Duration;
        
        _programRepository.Update(program);
    }

    public async Task DeleteAsync(int id)
    {
        var program = await _programRepository.GetOneAsync(p => p.Id == id);

        if (program == null)
        {
            throw new BaseException("Program not found");
        }
        
        _programRepository.Delete(program);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var program = await _programRepository.GetOneAsync(p => p.Id == id);

        if (program == null)
        {
            throw new BaseException("Program not found");
        }
        
        if (program.IsDeleted)
        {
            throw new BaseException("Program is already deleted.");
        }
        
        program.IsDeleted = true;
        _programRepository.Update(program);
    }

    public async Task RevertSoftDeleteAsync(int id)
    {
        var program = await _programRepository.GetOneAsync(p => p.Id == id);

        if (program == null)
        {
            throw new BaseException("Program not found");
        }
        
        if (!program.IsDeleted)
        {
            throw new BaseException("Program is already reverted.");
        }
        
        program.IsDeleted = false;
        _programRepository.Update(program);
    }

    public Task<int> SaveChangesAsync()
    {
        return _programRepository.SaveChangesAsync();
    }
}