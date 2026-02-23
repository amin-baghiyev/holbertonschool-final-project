namespace PLDMS.Core.Entities;

public class SessionExercise
{
    public Guid SessionId { get; set; }
    public Session Session { get; set; }
    
    public long ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
}