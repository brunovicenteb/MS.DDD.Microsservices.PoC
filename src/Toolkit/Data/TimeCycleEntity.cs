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
    public DateTime? UpdateAt { get; set; }
}