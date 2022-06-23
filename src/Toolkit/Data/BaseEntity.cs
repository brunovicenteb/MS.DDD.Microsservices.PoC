using Toolkit.Interfaces;

namespace Toolkit.Data;

public class BaseEntity : IIdentifiable
{
    public BaseEntity()
    {
    }
    public BaseEntity(uint id)
    {
        ID = id;
    }
    public uint ID { get; set; }
}