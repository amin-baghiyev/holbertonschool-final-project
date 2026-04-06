using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs.MentorDTOs;
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

	public async Task<(ICollection<MentorTableItemDTO> Items, int TotalCount)> MentorsAsTableItemAsync(string q, bool onlyActive = true, int page = 0, int count = 10)
	{
		var query = _userManager.Users.Where(u => u.Role == UserRole.Mentor && (!onlyActive || !u.IsDeleted));

		if (!string.IsNullOrWhiteSpace(q))
		{
			query = query.Where(u => EF.Functions.ILike(u.Email, $"%{q}%") || EF.Functions.ILike(u.FullName, $"%{q}%"));
		}

		var totalCount = await query.CountAsync();

		var items = await query
			.Skip(page * count)
			.Take(count)
			.Select(u => new MentorTableItemDTO
			{
				Id = u.Id,
				Email = u.Email,
				FullName = u.FullName,
				CreatedAt = u.CreatedAt,
				IsDeleted = u.IsDeleted
			})
			.OrderByDescending(u => u.CreatedAt)
			.ToListAsync();

		return (items, totalCount);
	}

	public async Task CreateAsync(MentorFormDTO dto)
	{
		var exists = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == _userManager.NormalizeEmail(dto.Email));
		if (exists) throw new BaseException("Email already exists");

		var user = new AppUser
		{
			Email = dto.Email,
			UserName = dto.Email,
			FullName = dto.FullName,
			Role = UserRole.Mentor,
			CreatedAt = DateTime.UtcNow.AddHours(4)
		};

		string randomPassword = Guid.NewGuid().ToString("N").Substring(0, 12);

		var result = await _userManager.CreateAsync(user, randomPassword);

		if (result.Succeeded)
		{
			_ = _emailService.SendEmailAsync(user.Email, "Your account has been created", $"Email: {dto.Email}<br>Password: {randomPassword}");
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
		var existingUser = await _userManager.FindByEmailAsync(dto.Email);
		if (existingUser is not null && existingUser.Id != id)
			throw new BaseException("Email already exists");

		var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new BaseException("Mentor not found");

		user.Email = dto.Email;
		user.UserName = dto.Email;
		user.FullName = dto.FullName;

		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
			throw new BaseException(string.Join(", ", result.Errors.Select(e => e.Description)));
	}

	public async Task SoftDeleteAsync(Guid id)
	{
		var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new BaseException("Mentor not found");

		user.IsDeleted = true;
		await _userManager.UpdateAsync(user);
	}

	public async Task RecoverAsync(Guid id)
	{
		var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new BaseException("Mentor not found");

		user.IsDeleted = false;
		await _userManager.UpdateAsync(user);
	}

	public Task<int> SaveChangesAsync()
	{
		return _mentorRepository.SaveChangesAsync();
	}
}