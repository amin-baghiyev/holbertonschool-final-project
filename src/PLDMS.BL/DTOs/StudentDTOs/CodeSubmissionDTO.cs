using System.ComponentModel.DataAnnotations;
using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record CodeSubmissionDTO
{
    public Guid GroupId { get; set; }
    
    [Required]
    public long ExerciseId { get; set; }
    
    [Required(ErrorMessage = "Source code is required.")]
    public string SourceCode { get; set; } = null!;
    
    [Required]
    public ProgrammingLanguage LanguageId { get; set; }
}
