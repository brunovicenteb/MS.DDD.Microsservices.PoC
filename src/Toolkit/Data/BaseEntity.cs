using Toolkit.Interfaces;

namespace Toolkit.Data;

public class BaseEntity : IIdentifiable
{
    public BaseEntity()
    {
    }
    public BaseEntity(uint id)
    {
    }
    public uint ID { get; set; }
}