using Toolkit.Exceptions;
using Toolkit.Interfaces;

namespace Toolkit.Data;

public class TimeCycleEntity : BaseEntity, ITimeCycle
{
    public TimeCycleEntity()
        : base()
    {
    }

    public TimeCycleEntity(uint id, DateTime createAt, DateTime? updateAt)
        : base(id)
    {
        CreateAt = createAt;
        UpdateAt = updateAt;
    }

    public DateTime CreateAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool IsActive => !DeletedAt.HasValue;

    public void Delete()
    {
        if (DeletedAt.HasValue)
            throw new DomainRuleException("Entity already deleted");
        DeletedAt = DateTime.UtcNow;
    }
}