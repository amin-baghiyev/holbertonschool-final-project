using FluentValidation;

namespace PLDMS.BL.DTOs.ProgramDTOs;

public record ProgramFormDTO
{
    public string Name { get; set; } = null!;
    public int Duration { get; set; }
}

public class ProgramValidator : AbstractValidator<ProgramFormDTO>
{
    public ProgramValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Program name is required")
            .MaximumLength(150)
            .WithMessage("Session name cannot exceed 150 characters");
        
        RuleFor(x => x.Duration)
            .NotEmpty()
            .WithMessage("Duration in months must be selected")
            .GreaterThan(0)
            .WithMessage("Duration in months must be greater than zero");
    }
}