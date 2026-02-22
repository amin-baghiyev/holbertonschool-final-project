namespace PLDMS.Core.Entities;

public class SessionTask
{
    public int SessionId { get; set; }
    public Session Session { get; set; }
    
    public int TaskId { get; set; }
    public Task Task { get; set; }
}