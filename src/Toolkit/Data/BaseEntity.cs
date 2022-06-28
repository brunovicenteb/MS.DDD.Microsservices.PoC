using FluentValidation;
using Toolkit.Interfaces;

namespace Toolkit.Data;

public abstract class BaseEntity : IIdentifiable
{
    public BaseEntity()
    {
    }
    public BaseEntity(uint id)
    {
        ID = id;
    }
    public uint ID { get; set; }

    public virtual IValidator[] GetValidators()
        => new IValidator[] { };
}