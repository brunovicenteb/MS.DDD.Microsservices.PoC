using Toolkit.Data;
using Toolkit.Exceptions;

namespace Benefit.Test.Toolkit.Data;
public class TimeCycleEntityTest
{
    #region NewTimeCycleEntityTest 
    private class NewTimeCycleEntityTest : TimeCycleEntity
    {
        public NewTimeCycleEntityTest() :
            base()
        {
        }
        public NewTimeCycleEntityTest(string id, DateTime createAt, DateTime? updateAt, DateTime? deleteAd)
            : base(id, createAt, updateAt, deleteAd)
        {
        }
    }
    #endregion

    [Fact]
    public void NewTimeCycleEntityTest_CreateWithoutParamsTest()
    {
        var newBaseEntity = new NewTimeCycleEntityTest();
        Assert.Equal(null as string, newBaseEntity.ID);
        Assert.Equal(DateTime.UtcNow.Year, newBaseEntity.CreateAt.Year);
        Assert.Equal(DateTime.UtcNow.Month, newBaseEntity.CreateAt.Month);
        Assert.Equal(DateTime.UtcNow.Day, newBaseEntity.CreateAt.Day);
    }

    [Fact]
    public void NewTimeCycleEntityTest_CreateWithParamsTest()
    {
        DateTime now = DateTime.Now;
        var newTimeCycleEntity = new NewTimeCycleEntityTest("25", now, null, null);
        Assert.Equal("25", newTimeCycleEntity.ID);
        Assert.Equal(now, newTimeCycleEntity.CreateAt);
        Assert.False(newTimeCycleEntity.UpdateAt.HasValue);
        Assert.False(newTimeCycleEntity.DeletedAt.HasValue);
    }

    [Fact]
    public void NewTimeCycleEntityTest_IsActive()
    {
        DateTime now = DateTime.Now;
        var entity = new NewTimeCycleEntityTest("25", now, null, null);
        Assert.True(entity.IsActive);

        entity = new NewTimeCycleEntityTest("25", now, null, DateTime.UtcNow);
        Assert.False(entity.IsActive);
    }

    [Fact]
    public void NewTimeCycleEntityTest_Delete()
    {
        DateTime now = DateTime.Now;
        var entity = new NewTimeCycleEntityTest("25", now, null, null);
        Assert.True(entity.IsActive);
        Assert.True(entity.Delete());
        Assert.False(entity.IsActive);
    }

    [Fact]
    public void NewTimeCycleEntityTest_DeleteAlreadyDeleted()
    {
        DateTime now = DateTime.Now;
        var entity = new NewTimeCycleEntityTest("25", now, null, DateTime.UtcNow);
        var exception = Assert.Throws<DomainRuleException>(() => entity.Delete());
        Assert.Equal("Entity already deleted.", exception.Message);

        entity = new NewTimeCycleEntityTest("25", now, null, null);
        entity.Delete();
        exception = Assert.Throws<DomainRuleException>(() => entity.Delete());
        Assert.Equal("Entity already deleted.", exception.Message);
    }

    [Fact]
    public void NewTimeCycleEntityTest_Update()
    {
        var entity = new NewTimeCycleEntityTest("25", DateTime.UtcNow, null, null);
        Assert.False(entity.UpdateAt.HasValue);

        entity.Update();
        Assert.True(entity.UpdateAt.HasValue);

        var moment = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
        TimeSpan sp = entity.UpdateAt.Value.Date.Subtract(moment);
        Assert.NotEqual(moment, entity.UpdateAt.Value);

        entity.Update(moment);
        Assert.Equal(moment, entity.UpdateAt.Value);
    }
}