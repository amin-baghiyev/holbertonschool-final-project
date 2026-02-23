using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace PLDMS.BL.DTOs;

public record UserLoginDTO
{
    [Display(Prompt = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Display(Prompt = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class UserLoginDTOValidator : AbstractValidator<UserLoginDTO>
{
    public UserLoginDTOValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty().WithMessage("Email can't be empty")
            .EmailAddress().WithMessage("Email is invalid");

        RuleFor(e => e.Password)
            .NotEmpty().WithMessage("Password can't be empty")
            .MinimumLength(6).WithMessage("Password must be at least 6 symbols length");
    }
}