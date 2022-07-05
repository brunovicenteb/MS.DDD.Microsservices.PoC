using FluentValidation;
using Toolkit.Interfaces;

namespace Toolkit.Data;

public abstract class BaseEntity : IIdentifiable
{
    public BaseEntity()
    {
    }
    public BaseEntity(string id)
    {
        ID = id;
    }
    public string ID { get; set; }

    public virtual IValidator[] GetValidators()
        => new IValidator[] { };
}