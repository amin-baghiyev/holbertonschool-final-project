namespace PLDMS.Core.Entities.Base;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; }
}