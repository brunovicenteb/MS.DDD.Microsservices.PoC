using Toolkit.Interfaces;

namespace Toolkit.Mapper;

public static class MapperFactory
{
    public static IGenericMapper Map<TSource, TDestination>()
    {
        return Nest<TSource, TDestination>() as IGenericMapper;
    }
    public static INestedMapper Nest<TSource, TDestination>()
    {
        var mapper = new NestedMapper();
        return mapper.Nest<TSource, TDestination>();
    }
}