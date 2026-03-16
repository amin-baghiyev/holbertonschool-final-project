using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using PLDMS.DL.Repositories.Abstractions;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using PLDMS.BL.Utilities;

namespace PLDMS.BL.Services.Concretes;

public class StudentService : IStudentService
{
    private readonly IRepository<Session> _sessionRepository;
    private readonly IRepository<Review> _reviewRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public StudentService(UserManager<AppUser> userManager, IEmailService emailService, IRepository<Session> sessionRepository, IRepository<Review> reviewRepository, IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
    }


    public async Task<(ICollection<StudentSessionListItemDTO> Items, int TotalCount)> GetSessionsAsync(
        Guid studentId, string? q = null, DateTime? startDate = null, DateTime? endDate = null, SessionStatus? status = null, int page = 0, int count = 25)
    {
        var now = DateTime.UtcNow;

        Expression<Func<Session, bool>> predicate = s =>
            s.Groups.Any(g => g.Students.Any(st => st.StudentId == studentId)) &&
            (string.IsNullOrWhiteSpace(q) || s.Name.Contains(q)) &&
            (!startDate.HasValue || s.StartDate >= startDate) &&
            (!endDate.HasValue || s.EndDate <= endDate) &&
            (
                !status.HasValue ||
                (status == SessionStatus.Upcoming && s.StartDate > now) ||
                (status == SessionStatus.Finished && s.EndDate < now) ||
                (status == SessionStatus.Active && s.StartDate <= now && s.EndDate >= now)
            );

        var (sessions, totalCount) = await _sessionRepository.GetAllAsync(
            predicate: predicate,
            page: page,
            count: count,
            orderAsc: false,
            orderBy: "CreatedAt",
            isTracking: false
        );

        return (_mapper.Map<ICollection<StudentSessionListItemDTO>>(sessions), totalCount);
    }

    public async Task<(ICollection<StudentReviewListItemDTO> Items, int TotalCount)> GetReviewsAsync(
        Guid studentId, ReviewStatus? status = null, int page = 0, int count = 25)
    {
        Expression<Func<Review, bool>> predicate = r =>
            r.ReviewerId == studentId &&
            (!status.HasValue || r.ReviewStatus == status);

        var (reviews, totalCount) = await _reviewRepository.GetAllAsync(
            predicate: predicate,
            page: page,
            count: count,
            includes: query => query.Include(r => r.Group).Include(r => r.AssignedBy),
            orderAsc: false,
            orderBy: "CreatedAt",
            isTracking: false
        );

        return (_mapper.Map<ICollection<StudentReviewListItemDTO>>(reviews), totalCount);
    }

    public async Task<StudentReviewCreateDTO?> GetReviewForEditAsync(Guid reviewId, Guid studentId)
    {
        var review = await _reviewRepository.GetOneAsync(
            predicate: r => r.Id == reviewId && r.ReviewerId == studentId,
            isTracking: false
        );

        if (review == null) return null;

        return new StudentReviewCreateDTO
        {
            Id = review.Id,
            Score = review.Score,
            Note = review.Note
        };
    }

    public async Task SubmitReviewAsync(Guid reviewId, Guid studentId, StudentReviewCreateDTO dto)
    {
        var review = await _reviewRepository.GetOneAsync(
            predicate: r => r.Id == reviewId && r.ReviewerId == studentId,
            isTracking: true
        );

        if (review == null)
            throw new BaseException("Review not found or not assigned to you.");

        if (review.ReviewStatus == ReviewStatus.Reviewed || review.ReviewStatus == ReviewStatus.Accepted || review.ReviewStatus == ReviewStatus.Rejected)
            throw new BaseException("This review has already been submitted or completed.");

        review.Score = dto.Score;
        review.Note = dto.Note;
        review.ReviewStatus = ReviewStatus.Reviewed;

        _reviewRepository.Update(review);
        await _reviewRepository.SaveChangesAsync();
    }

    public async Task<(ICollection<StudentTableItemDTO> Items, int TotalCount)> StudentsAsTableItemAsync(string q, int page, int count)
    {
        var query = _userManager.Users.Where(u => u.Role == UserRole.Student);
        
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u => u.Email.Contains(q, StringComparison.OrdinalIgnoreCase) 
                                  || u.FullName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .Skip(page * count)
            .Take(count)
            .Select(u => new StudentTableItemDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                UserName =  u.UserName,
                Email = u.Email
            }).ToListAsync();

        return (items, totalCount);
    }

    public async Task CreateAsync(StudentFormDTO dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null) throw new BaseException("Email already exists.");
        
        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            Role = UserRole.Student
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
            throw new BaseException($"Failed to create student: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdateAsync(Guid id, StudentFormDTO dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new BaseException("User not found.");

        user.Email = dto.Email;
        user.FullName = dto.FullName;
        user.UserName = dto.UserName;

        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        
        if (user == null) throw new BaseException("User not found.");
        
        await _userManager.DeleteAsync(user);
    }

    public Task<int> SaveChangesAsync() => Task.FromResult(0);
}