namespace Toolkit.Interfaces;

public interface ITimeCycle
{
    DateTime CreateAt { get; set; }
    DateTime? DeletedAt { get; set; }
    DateTime? UpdateAt { get; set; }
    bool IsActive { get; }
    void Delete();
}