using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs.MentorDTOs;
using PLDMS.BL.Extension;
using PLDMS.BL.Services.Abstractions;
using PLDMS.BL.Utilities;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using PLDMS.DL.Repositories.Abstractions;

namespace PLDMS.BL.Services.Concretes;

public class MentorService : IMentorService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IRepository<AppUser> _mentorRepository;

    public MentorService(UserManager<AppUser> userManager, IRepository<AppUser> mentorRepository, IEmailService emailService)
    {
        _userManager = userManager;
        _mentorRepository = mentorRepository;
        _emailService = emailService;
    }

    public async Task<(ICollection<MentorTableItemDTO> Items, int TotalCount)> MentorsAsTableItemAsync(string q, int page = 1, int count = 10)
    {
        var query = _userManager.Users.Where(u => u.Role == UserRole.Mentor);
        
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u => u.Email.Contains(q, StringComparison.OrdinalIgnoreCase) 
                                  || u.UserName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(page * count)
            .Take(count)
            .Select(u => new MentorTableItemDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                FullName = u.FullName
            }).ToListAsync();

        return (items, totalCount);
    }

    public async Task CreateAsync(MentorFormDTO dto)
    {
        var existingUser = _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null) throw new BaseException("Email already exists.");

        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            Role = UserRole.Mentor
        };

        string randomPassword = Guid.NewGuid().ToString("N").Substring(0, 12);

        var result = await _userManager.CreateAsync(user, randomPassword);
        
        if (result.Succeeded)
        {
            _ = _emailService.SendEmailAsync(user.Email, "test", randomPassword);
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Password: {randomPassword}");
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