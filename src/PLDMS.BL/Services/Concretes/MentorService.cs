using Microsoft.AspNetCore.Identity;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs.MentorDTOs;
using PLDMS.BL.Extension;
using PLDMS.BL.Services.Abstractions;
using PLDMS.BL.Utilities;
using PLDMS.Core.Entities;
using PLDMS.DL.Repositories.Abstractions;

namespace PLDMS.BL.Services.Concretes;

public class MentorService : IMentorService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IRepository<AppUser> _mentorRepository;
    private const string MentorRole = "Mentor";

    public MentorService(UserManager<AppUser> userManager, IRepository<AppUser> mentorRepository, IEmailService emailService)
    {
        _userManager = userManager;
        _mentorRepository = mentorRepository;
        _emailService = emailService;
    }

    public async Task<(ICollection<MentorTableItemDTO> Items, int TotalCount)> MentorsAsTableItemAsync(string q, int page = 1, int count = 10)
    {
        var mentors = await _userManager.GetUsersInRoleAsync(MentorRole);

        var query = mentors.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u => u.Email.Contains(q, StringComparison.OrdinalIgnoreCase) 
                                  || u.UserName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * count)
            .Take(count)
            .Select(u => new MentorTableItemDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                FullName = u.FullName
            })
            .ToList();

        return (items, totalCount);
    }

    public async Task CreateAsync(MentorFormDTO dto)
    {
        await _userManager.ThrowIfInRoleAsync(dto.Email, MentorRole); 

        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
        };

        string randomPassword = Guid.NewGuid().ToString("N").Substring(0, 12);

        var result = await _userManager.CreateAsync(user, randomPassword);
        
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, MentorRole);
            await _emailService.SendEmailAsync(user.Email, "test", randomPassword);
        }
        else
        {
            throw new BaseException($"Failed to create mentor: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdateAsync(Guid id, MentorFormDTO dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new BaseException("Mentor not found.");

        user.Email = dto.Email;
        user.UserName = dto.UserName;
        user.FullName = dto.FullName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BaseException("Update failed.");
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        
        if (user == null)
        {
            throw new BaseException("Mentor not found.");
        }
        
        await _userManager.DeleteAsync(user);
    }

    public Task<int> SaveChangesAsync()
    {
        return _mentorRepository.SaveChangesAsync();
    }
}