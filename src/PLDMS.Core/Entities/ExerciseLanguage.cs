using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class ExerciseLanguage
{
    public long ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
    
    public ProgrammingLanguage ProgrammingLanguage { get; set; }
}