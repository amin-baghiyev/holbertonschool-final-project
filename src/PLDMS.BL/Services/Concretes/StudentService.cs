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
	private readonly IRepository<Group> _groupRepository;
	private readonly IRepository<Submission> _submissionRepository;
	private readonly UserManager<AppUser> _userManager;
	private readonly IEmailService _emailService;
	private readonly IMapper _mapper;

	public StudentService(UserManager<AppUser> userManager, IEmailService emailService, IRepository<Session> sessionRepository, IRepository<Review> reviewRepository, IRepository<Group> groupRepository, IRepository<Submission> submissionRepository, IMapper mapper)
	{
		_sessionRepository = sessionRepository;
		_reviewRepository = reviewRepository;
		_groupRepository = groupRepository;
		_submissionRepository = submissionRepository;
		_userManager = userManager;
		_emailService = emailService;
		_mapper = mapper;
	}

	public async Task<(ICollection<StudentSessionListItemDTO> Items, int TotalCount)> GetSessionsAsync(
		Guid studentId, string? q = null, DateTime? startDate = null, DateTime? endDate = null, SessionStatus? status = null, int page = 0, int count = 25, int? cohortId = null, int? programId = null)
	{
		var now = DateTime.UtcNow.AddHours(4);

		Expression<Func<Session, bool>> predicate = s =>
			s.Groups.Any(g => g.Students.Any(st => st.StudentId == studentId)) &&
			(string.IsNullOrWhiteSpace(q) || s.Name.ToLower().Contains(q.ToLower())) &&
			(!startDate.HasValue || s.StartDate >= startDate) &&
			(!endDate.HasValue || s.EndDate <= endDate) &&
			(!cohortId.HasValue || s.CohortId == cohortId) &&
			(!programId.HasValue || (s.Cohort != null && s.Cohort.ProgramId == programId)) &&
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
			includes: query => query
				.Include(s => s.Cohort).ThenInclude(c => c.Program)
				.Include(s => s.Exercises)
				.Include(s => s.Groups).ThenInclude(g => g.Students)
				.Include(s => s.Groups).ThenInclude(g => g.Submissions),
			orderAsc: false,
			orderBy: "CreatedAt",
			isTracking: false
		);

		var dtos = _mapper.Map<ICollection<StudentSessionListItemDTO>>(sessions);

		foreach (var dto in dtos)
		{
			var session = sessions.First(s => s.Id == dto.Id);
			var group = session.Groups.FirstOrDefault(g => g.Students.Any(st => st.StudentId == studentId));
			if (group != null)
			{
				dto.GroupName = group.Name;
				dto.GroupStudentCount = group.TotalStudentCount;
				dto.ExercisesCount = session.Exercises.Count;
				dto.SolvedExercisesCount = session.Exercises
					.Count(se => group.Submissions
						.Any(sub => sub.ExerciseId == se.ExerciseId && sub.Tests.Length > 0 && sub.Tests.All(t => t)));
			}
		}

		return (dtos, totalCount);
	}

	public async Task<(ICollection<CohortOptionItemDTO> Cohorts, ICollection<ProgramOptionItemDTO> Programs)> MySessionFilterOptionsAsync(Guid studentId)
	{
		var (sessions, _) = await _sessionRepository.GetAllAsync(
			predicate: s => s.Groups.Any(g => g.Students.Any(st => st.StudentId == studentId)),
			includes: query => query.Include(s => s.Cohort).ThenInclude(c => c.Program),
			page: 0,
			count: 1000,
			isTracking: false
		);

		var cohorts = sessions
			.Where(s => s.Cohort != null)
			.Select(s => s.Cohort)
			.DistinctBy(c => c.Id)
			.Select(c => new CohortOptionItemDTO { Id = c.Id, Name = c.Name })
			.ToList();

		var programs = sessions
			.Where(s => s.Cohort?.Program != null)
			.Select(s => s.Cohort.Program)
			.DistinctBy(p => p.Id)
			.Select(p => new ProgramOptionItemDTO { Id = p.Id, Name = p.Name })
			.ToList();

		return (cohorts, programs);
	}

	public async Task<StudentSessionDetailDTO> GetSessionDetailAsync(Guid sessionId, Guid studentId)
	{
		var session = await _sessionRepository.GetOneAsync(
			predicate: s => s.Id == sessionId && s.Groups.Any(g => g.Students.Any(st => st.StudentId == studentId)),
			includes: query => query
				.Include(s => s.Cohort).ThenInclude(c => c.Program)
				.Include(s => s.Exercises).ThenInclude(se => se.Exercise).ThenInclude(e => e.ExerciseLanguages),
			isTracking: false);

		if (session == null)
			throw new BaseException("Session not found or not assigned to you");

		var group = await _groupRepository.GetOneAsync(
			predicate: g => g.SessionId == sessionId && g.Students.Any(s => s.StudentId == studentId),
			includes: query => query.Include(g => g.Students).ThenInclude(sg => sg.Student),
			isTracking: false);

		var dto = _mapper.Map<StudentSessionDetailDTO>(session);
		if (group != null)
		{
			dto.AssignedGroupId = group.Id;
			dto.AssignedGroupName = group.Name;
			dto.Teammates = group.Students
				.Where(sg => sg.StudentId != studentId)
				.Select(sg => _mapper.Map<TeammateDTO>(sg.Student))
				.ToList();

			// Fetch best submission (most passes) for each exercise for this group
			var submissions = await _submissionRepository.Table
				.Where(s => s.GroupId == group.Id && session.Exercises.Select(e => e.ExerciseId).Contains(s.ExerciseId))
				.ToListAsync();

			foreach (var exerciseDto in dto.Exercises)
			{
				var exerciseSubmissions = submissions.Where(s => s.ExerciseId == exerciseDto.Id).ToList();
				if (exerciseSubmissions.Any())
				{
					var bestSubmission = exerciseSubmissions
						.OrderByDescending(s => s.Tests.Count(t => t))
						.First();

					exerciseDto.PassCount = bestSubmission.Tests.Count(t => t);
					exerciseDto.TotalTests = bestSubmission.Tests.Length;
					exerciseDto.IsSolved = exerciseDto.PassCount == exerciseDto.TotalTests;
				}
			}
		}

		return dto;
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
			throw new BaseException("Review not found or not assigned to you");

		if (review.ReviewStatus == ReviewStatus.Reviewed || review.ReviewStatus == ReviewStatus.Accepted || review.ReviewStatus == ReviewStatus.Rejected)
			throw new BaseException("This review has already been submitted or completed");

		review.Score = dto.Score;
		review.Note = dto.Note;
		review.ReviewStatus = ReviewStatus.Reviewed;

		_reviewRepository.Update(review);
		await _reviewRepository.SaveChangesAsync();
	}

	public async Task<(ICollection<StudentTableItemDTO> Items, int TotalCount)> StudentsAsTableItemAsync(string q, bool onlyActive = true, int page = 0, int count = 25)
	{
		var query = _userManager.Users.Where(u => u.Role == UserRole.Student && (!onlyActive || !u.IsDeleted));

		if (!string.IsNullOrWhiteSpace(q))
		{
			query = query.Where(u => EF.Functions.ILike(u.Email, $"%{q}%") || EF.Functions.ILike(u.FullName, $"%{q}%"));
		}

		var totalCount = await query.CountAsync();

		var items = await query
			.Skip(page * count)
			.Take(count)
			.Select(u => new StudentTableItemDTO
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

	public async Task CreateAsync(StudentFormDTO dto)
	{
		var exists = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == _userManager.NormalizeEmail(dto.Email));
		if (exists) throw new BaseException("Email already exists");

		var user = new AppUser
		{
			Email = dto.Email,
			UserName = dto.Email,
			FullName = dto.FullName,
			Role = UserRole.Student,
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
			throw new BaseException($"Failed to create student: {string.Join(", ", result.Errors.Select(e => e.Description))}");
		}
	}

	public async Task UpdateAsync(Guid id, StudentFormDTO dto)
	{
		var existingUser = await _userManager.FindByEmailAsync(dto.Email);
		if (existingUser is not null && existingUser.Id != id)
			throw new BaseException("Email already exists");

		var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new BaseException("Student not found");

		user.Email = dto.Email;
		user.UserName = dto.Email;
		user.FullName = dto.FullName;

		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
			throw new BaseException(string.Join(", ", result.Errors.Select(e => e.Description)));
	}

	public async Task SoftDeleteAsync(Guid id)
	{
		var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new BaseException("Student not found");

		user.IsDeleted = true;
		await _userManager.UpdateAsync(user);
	}

	public async Task RecoverAsync(Guid id)
	{
		var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new BaseException("Student not found");

		user.IsDeleted = false;
		await _userManager.UpdateAsync(user);
	}
}