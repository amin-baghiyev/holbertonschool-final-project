using FluentValidation;
using PLDMS.BL.DTOs.ProgramDTOs;

namespace PLDMS.BL.DTOs.CohortDTOs;

public record CohortFormDTO
{
    public string Name { get; set; }
    public int ProgramId { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}

public class CohortValidator : AbstractValidator<CohortFormDTO>
{
    public CohortValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Cohort name is required")
            .MaximumLength(150)
            .WithMessage("Session name cannot exceed 150 characters");
        
        RuleFor(x => x.ProgramId)
            .GreaterThan(0)
            .WithMessage("Program must be selected");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}