using Toolkit.Exceptions;
using Toolkit.Interfaces;

namespace Toolkit.Data;

public abstract class TimeCycleEntity : BaseEntity, ITimeCycle
{
    public TimeCycleEntity()
        : base()
    {
        CreateAt = DateTime.UtcNow;
    }

    public TimeCycleEntity(string id, DateTime createAt, DateTime? updateAt, DateTime? deletedAt)
        : base(id)
    {
        CreateAt = createAt;
        UpdateAt = updateAt;
        DeletedAt = deletedAt;
    }

    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive => !DeletedAt.HasValue;

    public bool Update(DateTime? updateAt = null)
    {
        UpdateAt = updateAt ?? DateTime.UtcNow;
        return true;
    }

    public bool Delete()
    {
        if (DeletedAt.HasValue)
            throw new DomainRuleException("Entity already deleted.");
        DeletedAt = DateTime.UtcNow;
        return true;
    }
}