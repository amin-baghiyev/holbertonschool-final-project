using FluentValidation;

namespace PLDMS.BL.DTOs;

public record StudentFormDTO
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
}

public class StudentFormValidator : AbstractValidator<StudentFormDTO>
{
    public StudentFormValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username is too short.");
    }
}