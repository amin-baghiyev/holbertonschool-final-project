namespace PLDMS.Core.Entities;

public class SessionTask
{
    public Guid SessionId { get; set; }
    public Session Session { get; set; }
    
    public long TaskId { get; set; }
    public Task Task { get; set; }
}