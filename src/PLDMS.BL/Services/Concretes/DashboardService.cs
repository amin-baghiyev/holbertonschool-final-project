using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.DTOs.DashboardDTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using PLDMS.DL.Contexts;

namespace PLDMS.BL.Services.Concretes;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DashboardService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<AdminDashboardStatsDTO> AdminDashboardStatsAsync()
    {
        var totalStudents = await _userManager.Users.CountAsync(u => u.Role == UserRole.Student && !u.IsDeleted);
        var totalMentors = await _userManager.Users.CountAsync(u => u.Role == UserRole.Mentor && !u.IsDeleted);
        
        var newRegs = await _userManager.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddHours(4).AddDays(-30) && !u.IsDeleted);

        var activeCohorts = await _context.Cohorts.CountAsync(c => !c.IsDeleted);
        var activePrograms = await _context.Programs.CountAsync(p => !p.IsDeleted);

        var stats = new AdminDashboardStatsDTO
        {
            TotalUsers = totalStudents + totalMentors,
            TotalStudents = totalStudents,
            TotalMentors = totalMentors,
            ActiveCohorts = activeCohorts,
            ActivePrograms = activePrograms,
            NewRegistrations = newRegs,
            RecentActivities = new List<SystemActivityDTO>()
        };

        var recentMentors = await _userManager.Users
            .Where(u => u.Role == UserRole.Mentor)
            .OrderByDescending(u => u.CreatedAt)
            .Take(2)
            .ToListAsync();

        foreach (var m in recentMentors)
        {
            stats.RecentActivities.Add(new SystemActivityDTO
            {
                Type = "Mentor",
                ActionTextHtml = $"New Mentor <span class=\"font-semibold\">{m.FullName}</span> added to system",
                SubText = $"Added on {m.CreatedAt:MMM dd}",
                IconClass = "ph-user-plus",
                BgClass = "bg-brand-red",
                TextColorClass = "text-white",
                Timestamp = m.CreatedAt
            });
        }

        var recentStudents = await _userManager.Users
            .Where(u => u.Role == UserRole.Student)
            .OrderByDescending(u => u.CreatedAt)
            .Take(2)
            .ToListAsync();

        foreach (var s in recentStudents)
        {
            stats.RecentActivities.Add(new SystemActivityDTO
            {
                Type = "Student",
                ActionTextHtml = $"New Student <span class=\"font-semibold\">{s.FullName}</span> joined",
                SubText = $"Registered on {s.CreatedAt:MMM dd}",
                IconClass = "ph-user-circle-plus",
                BgClass = "bg-green-100",
                TextColorClass = "text-green-600",
                Timestamp = s.CreatedAt
            });
        }

        var recentCohorts = await _context.Cohorts
            .Include(c => c.Program)
            .OrderBy(c => c.Id)
            .Take(2)
            .ToListAsync();

        foreach (var c in recentCohorts)
        {
            stats.RecentActivities.Add(new SystemActivityDTO
            {
                Type = "Cohort",
                ActionTextHtml = $"Cohort <span class=\"font-medium\">{c.Name}</span> created for {c.Program?.Name}",
                SubText = $"Starts on {c.StartDate:MMM dd}",
                IconClass = "ph-stack",
                BgClass = "bg-purple-100",
                TextColorClass = "text-purple-600",
                Timestamp = DateTime.UtcNow.AddHours(4)
            });
        }

        stats.RecentActivities = stats.RecentActivities.OrderByDescending(a => a.Timestamp).Take(5).ToList();

        return stats;
    }
}
