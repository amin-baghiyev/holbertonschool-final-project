using FluentValidation;
using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record ExerciseFormDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public int ProgramId { get; set; }
    public IEnumerable<ExerciseTestCasesDTO> TestCases { get; set; } = [];
    public IEnumerable<ProgrammingLanguage> Languages { get; set; } = [];
}

public class ExerciseFormDTOValidator : AbstractValidator<ExerciseFormDTO>
{
    public ExerciseFormDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Invalid difficulty level");

        RuleFor(x => x.ProgramId)
            .GreaterThan(0).WithMessage("A valid Program must be selected");

        RuleFor(x => x.Languages)
            .NotEmpty().WithMessage("At least one programming language must be selected");

        RuleForEach(x => x.Languages)
            .IsInEnum().WithMessage("Invalid programming language");

        RuleFor(x => x.TestCases)
            .NotEmpty().WithMessage("At least one test case is required");

        RuleForEach(x => x.TestCases).ChildRules(testCase =>
        {
            testCase.RuleFor(tc => tc.Input)
                .NotEmpty().WithMessage("Test case input cannot be empty");

            testCase.RuleFor(tc => tc.Output)
                .NotEmpty().WithMessage("Test case output cannot be empty");
        });
    }
}