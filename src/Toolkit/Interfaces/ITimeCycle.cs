namespace Toolkit.Interfaces;

public interface ITimeCycle
{
    DateTime CreateAt { get; set; }

    DateTime? UpdateAt { get; set; }
}