using Toolkit.Data;

namespace Toolkit.Test.Data;

public class BaseEntityTest
{
    #region NewBaseEntityTest
    private class NewBaseEntityTest : BaseEntity
    {
        public NewBaseEntityTest() :
            base()
        {
        }
        public NewBaseEntityTest(uint id) :
            base(id)
        {
        }
    }
    #endregion

    [Fact]
    public void CreatorWithoutParamsTest()
    {
        var newBaseEntity = new NewBaseEntityTest();
        Assert.Equal((uint)0, newBaseEntity.ID);
    }

    [Fact]
    public void CreatorWithParamsTest()
    {
        var newBaseEntity = new NewBaseEntityTest(25);
        Assert.Equal((uint)25, newBaseEntity.ID);
    }


    [Fact]
    public void GetValidatorsTest()
    {
        var newBaseEntity = new NewBaseEntityTest();
        var validators = newBaseEntity.GetValidators();
        Assert.NotNull(validators);
        Assert.Empty(validators);
    }
}