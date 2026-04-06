using FluentValidation;

namespace PLDMS.BL.DTOs.MentorDTOs;

public record MentorFormDTO
{
    public string Email { get; set; }
    public string FullName { get; set; }
}

public class MentorFormValidator : AbstractValidator<MentorFormDTO>
{
    public MentorFormValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MinimumLength(3).WithMessage("Full Name must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}