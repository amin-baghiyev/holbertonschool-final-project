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

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Group> _groupRepository;
    private readonly IRepository<Submission> _submissionRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly ISubmissionService _submissionService;
    private readonly IMapper _mapper;

    public ReviewService(IRepository<Review> reviewRepository, IRepository<Group> groupRepository, IRepository<Submission> submissionRepository, IRepository<Session> sessionRepository, ISubmissionService submissionService, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _groupRepository = groupRepository;
        _submissionRepository = submissionRepository;
        _sessionRepository = sessionRepository;
        _submissionService = submissionService;
        _mapper = mapper;
    }

    public async Task<(ICollection<MentorReviewListItemDTO> Items, int TotalCount)> GetMentorReviewsAsync(string? q = null, ReviewStatus? status = null, int page = 0, int count = 25)
    {
        Expression<Func<Review, bool>> predicate = r => 
            (!status.HasValue || r.ReviewStatus == status) &&
            (string.IsNullOrEmpty(q) || EF.Functions.ILike(r.Group.Name, $"%{q}%") || EF.Functions.ILike(r.Group.Session.Name, $"%{q}%"));

        var (reviews, totalCount) = await _reviewRepository.GetAllAsync(
            predicate: predicate,
            page: page,
            count: count,
            includes: query => query
                .Include(r => r.Reviewer)
                .Include(r => r.AssignedBy)
                .Include(r => r.Group).ThenInclude(g => g.Session),
            orderAsc: false,
            orderBy: "CreatedAt",
            isTracking: false
        );

        return (_mapper.Map<ICollection<MentorReviewListItemDTO>>(reviews), totalCount);
    }

    public async Task<(ICollection<GroupForReviewDTO> Items, int TotalCount)> GetGroupsNeedingReviewAsync(
        string? q = null, 
        ReviewStatus? status = null, 
        int? cohortId = null, 
        int? programId = null, 
        int page = 0, 
        int count = 25)
    {
        var now = DateTime.UtcNow.AddHours(4);

        var query = _groupRepository.Table
            .Include(g => g.Session).ThenInclude(s => s.Cohort).ThenInclude(c => c.Program)
            .Where(g => g.Session.EndDate <= now);

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(g => EF.Functions.ILike(g.Name, $"%{q}%") || EF.Functions.ILike(g.Session.Name, $"%{q}%"));
        }

        if (cohortId.HasValue) query = query.Where(g => g.Session.CohortId == cohortId);
        if (programId.HasValue) query = query.Where(g => g.Session.Cohort.ProgramId == programId);

        var joinedQuery = from g in query
                          join r in _reviewRepository.Table.Include(r => r.Reviewer) on g.Id equals r.GroupId into reviews
                          from r in reviews.DefaultIfEmpty()
                          select new { Group = g, ReviewStatus = (ReviewStatus?)r.ReviewStatus, ReviewerFullName = r != null ? r.Reviewer.FullName : null };

        if (status.HasValue)
        {
            if (status.Value == (ReviewStatus)(-1))
            {
                joinedQuery = joinedQuery.Where(x => x.ReviewStatus == null);
            }
            else if (status.Value == ReviewStatus.Reviewed)
            {
                joinedQuery = joinedQuery.Where(x => x.ReviewStatus == ReviewStatus.Reviewed || 
                                                   x.ReviewStatus == ReviewStatus.Accepted || 
                                                   x.ReviewStatus == ReviewStatus.Rejected);
            }
            else
            {
                joinedQuery = joinedQuery.Where(x => x.ReviewStatus == status.Value);
            }
        }
        else
        {
            joinedQuery = joinedQuery.Where(x => x.ReviewStatus == null);
        }

        var totalCount = await joinedQuery.CountAsync();
        var items = await joinedQuery
            .OrderByDescending(x => x.Group.Session.EndDate)
            .Skip(page * count)
            .Take(count)
            .ToListAsync();

        var dtos = items.Select(x => new GroupForReviewDTO
        {
            GroupId = x.Group.Id,
            GroupName = x.Group.Name,
            SessionId = x.Group.SessionId,
            SessionName = x.Group.Session.Name,
            CohortName = x.Group.Session.Cohort.Name,
            ProgramName = x.Group.Session.Cohort.Program.Name,
            SessionStartDate = x.Group.Session.StartDate,
            SessionEndDate = x.Group.Session.EndDate,
            ReviewStatus = x.ReviewStatus,
            ReviewerFullName = x.ReviewerFullName
        }).ToList();

        return (dtos, totalCount);
    }

    public async Task AssignReviewAsync(Guid mentorId, MentorAssignReviewDTO dto)
    {
        var group = await _groupRepository.GetOneAsync(
            g => g.Id == dto.GroupId,
            includes: query => query.Include(g => g.Session),
            isTracking: false);

        if (group == null) throw new BaseException("Group not found");
        if (group.Session.EndDate > DateTime.UtcNow.AddHours(4))
            throw new BaseException("Cannot assign review before the session ends.");

        var existingReview = await _reviewRepository.GetOneAsync(r => r.GroupId == dto.GroupId, isTracking: true);
        
        if (existingReview != null)
        {
            if (existingReview.ReviewStatus != ReviewStatus.Pending)
                throw new BaseException("This group has already been reviewed.");

            existingReview.ReviewerId = dto.ReviewerId;
            existingReview.AssignedById = mentorId;
            existingReview.CreatedAt = DateTime.UtcNow.AddHours(4);
            
            _reviewRepository.Update(existingReview);
        }
        else
        {
            var review = new Review
            {
                Id = Guid.CreateVersion7(),
                GroupId = dto.GroupId,
                ReviewerId = dto.ReviewerId,
                AssignedById = mentorId,
                ReviewStatus = ReviewStatus.Pending,
                Score = 0,
                Note = "Assigned",
                CreatedAt = DateTime.UtcNow.AddHours(4)
            };

            await _reviewRepository.CreateAsync(review);
        }

        await _reviewRepository.SaveChangesAsync();
    }

    public async Task GiveReviewAsync(Guid mentorId, MentorReviewCreateDTO dto)
    {
        var group = await _groupRepository.GetOneAsync(
            g => g.Id == dto.GroupId,
            includes: query => query.Include(g => g.Session),
            isTracking: false);

        if (group == null) throw new BaseException("Group not found");
        if (group.Session.EndDate > DateTime.UtcNow.AddHours(4))
            throw new BaseException("Cannot give review before the session ends.");

        var existingReview = await _reviewRepository.GetOneAsync(r => r.GroupId == dto.GroupId, isTracking: true);
        
        if (existingReview != null)
        {
            if (existingReview.ReviewStatus != ReviewStatus.Pending)
                throw new BaseException("This group has already been reviewed.");

            existingReview.ReviewerId = mentorId;
            existingReview.ReviewStatus = ReviewStatus.Reviewed;
            existingReview.Score = dto.Score;
            existingReview.Note = dto.Note;
            existingReview.CreatedAt = DateTime.UtcNow.AddHours(4);
            
            _reviewRepository.Update(existingReview);
        }
        else
        {
            var review = new Review
            {
                Id = Guid.CreateVersion7(),
                GroupId = dto.GroupId,
                ReviewerId = mentorId,
                AssignedById = mentorId,
                ReviewStatus = ReviewStatus.Reviewed,
                Score = dto.Score,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow.AddHours(4)
            };

            await _reviewRepository.CreateAsync(review);
        }

        await _reviewRepository.SaveChangesAsync();
    }

    public async Task UpdateMentorReviewStatusAsync(Guid reviewId, ReviewStatus status)
    {
        var review = await _reviewRepository.GetOneAsync(r => r.Id == reviewId, isTracking: true);
        if (review == null) throw new BaseException("Review not found");

        review.ReviewStatus = status;
        _reviewRepository.Update(review);
        await _reviewRepository.SaveChangesAsync();
    }

    public async Task<ReviewSummaryDTO> GetReviewSummaryAsync()
    {
        var now = DateTime.UtcNow.AddHours(4);

        var total = await _reviewRepository.Table.CountAsync();
        var pending = await _reviewRepository.Table.CountAsync(r => r.ReviewStatus == ReviewStatus.Pending);
        var completed = await _reviewRepository.Table.CountAsync(r => r.ReviewStatus != ReviewStatus.Pending);

        var reviewedGroupIds = await _reviewRepository.Table.Select(r => r.GroupId).ToListAsync();
        var needingReview = await _groupRepository.Table.CountAsync(g => g.Session.EndDate <= now && !reviewedGroupIds.Contains(g.Id));

        return new ReviewSummaryDTO
        {
            TotalReviews = total,
            PendingReviews = pending,
            CompletedReviews = completed,
            GroupsNeedingReview = needingReview
        };
    }

    public async Task<ReviewDetailDTO> GetReviewDetailAsync(Guid reviewId)
    {
        var review = await _reviewRepository.GetOneAsync(
            r => r.Id == reviewId,
            includes: query => query.Include(r => r.Group).ThenInclude(g => g.Session),
            isTracking: false);

        if (review == null) throw new BaseException("Review not found");

        return await BuildReviewDetailAsync(review, review.Group);
    }

    public async Task<ReviewDetailDTO> GetReviewDetailForGroupAsync(Guid groupId)
    {
        var review = await _reviewRepository.GetOneAsync(
            r => r.GroupId == groupId,
            includes: query => query.Include(r => r.Group).ThenInclude(g => g.Session),
            isTracking: false);

        if (review == null)
        {
            var group = await _groupRepository.GetOneAsync(
                g => g.Id == groupId,
                includes: query => query.Include(g => g.Session),
                isTracking: false);

            if (group == null) throw new BaseException("Group not found");

            return await BuildReviewDetailAsync(null, group);
        }

        return await BuildReviewDetailAsync(review, review.Group);
    }

    private async Task<ReviewDetailDTO> BuildReviewDetailAsync(Review? review, Group group)
    {
        var session = group.Session;

        var allGroupSubmissions = await _submissionRepository.Table
            .Where(s => s.GroupId == group.Id)
            .Include(s => s.Exercise)
            .ToListAsync();

        var submissionDtos = new List<SubmissionForReviewDTO>();

        var groupedByExercise = allGroupSubmissions.GroupBy(s => s.ExerciseId);

        foreach (var exerciseGroup in groupedByExercise)
        {
            var successfulSubmissions = exerciseGroup.Where(s => s.Tests != null && s.Tests.Length > 0 && s.Tests.All(t => t)).ToList();

            Submission? bestSubmission = null;

            if (successfulSubmissions.Any())
            {
                bestSubmission = successfulSubmissions.OrderByDescending(s => s.CreatedAt).First();
            }
            else if (exerciseGroup.Any())
            {
                bestSubmission = exerciseGroup.OrderByDescending(s => s.CreatedAt).First();
            }

            if (bestSubmission != null)
            {
                var sourceCode = await _submissionService.GetLastSubmittedCodeAsync(group.Id, bestSubmission.ExerciseId);
                
                submissionDtos.Add(new SubmissionForReviewDTO
                {
                    ExerciseId = bestSubmission.ExerciseId,
                    ExerciseName = bestSubmission.Exercise?.Name ?? "Unknown",
                    Description = bestSubmission.Exercise?.Description ?? "No description available",
                    CommitHash = bestSubmission.CommitHash,
                    BranchName = group.Name,
                    Language = bestSubmission.ProgrammingLanguage,
                    CreatedAt = bestSubmission.CreatedAt,
                    SourceCode = sourceCode ?? "// Code not found in GitHub",
                    Tests = bestSubmission.Tests
                });
            }
        }

        return new ReviewDetailDTO
        {
            ReviewId = review?.Id ?? Guid.Empty,
            GroupId = group.Id,
            GroupName = group.Name,
            SessionId = session.Id,
            SessionName = session.Name,
            RepositoryUrl = session.RepositoryUrl,
            ReviewerId = review?.ReviewerId,
            Score = review?.Score ?? 0,
            Note = review?.Note ?? string.Empty,
            Status = review?.ReviewStatus ?? ReviewStatus.Pending,
            Submissions = submissionDtos
        };
    }
}
