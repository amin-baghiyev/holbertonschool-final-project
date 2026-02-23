using FluentValidation;

namespace PLDMS.BL.DTOs;

public record SessionFormDTO
{
    public string Name { get; set; } = null!;
    public int CohortId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int StudentCountPerGroup { get; set; }
    public ICollection<Guid> ExercisesIds { get; set; } = [];
}

public class SessionFormDTOValidator : AbstractValidator<SessionFormDTO>
{
    public SessionFormDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Session name is required")
            .MaximumLength(150)
            .WithMessage("Session name cannot exceed 150 characters");

        RuleFor(x => x.CohortId)
            .GreaterThan(0)
            .WithMessage("A valid cohort must be selected");

        RuleFor(x => x.StudentCountPerGroup)
            .GreaterThan(0)
            .WithMessage("Student count per group must be greater than zero");

        RuleFor(x => x.ExercisesIds)
            .NotNull()
            .WithMessage("At least one exercise must be selected");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be later than the start date");
    }
}