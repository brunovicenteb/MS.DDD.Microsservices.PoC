using FluentValidation;
using Toolkit.Interfaces;

namespace Toolkit.Data;

public abstract class BaseEntity : IIdentifiable
{
    public BaseEntity()
    {
    }
    public BaseEntity(int id)
    {
        ID = id;
    }
    public int ID { get; set; }

    public virtual IValidator[] GetValidators()
        => new IValidator[] { };
}