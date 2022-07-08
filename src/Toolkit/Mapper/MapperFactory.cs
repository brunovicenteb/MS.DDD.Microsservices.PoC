using Toolkit.Interfaces;

namespace Toolkit.Mapper;

public static class MapperFactory
{
    public static IGenericMapper Map<TSource, TDestination>()
    {
        return new GenericMapper<TSource, TDestination>();
    }
    public static INestedMapper Nest<TSource, TDestination>()
    {
        var mapper = new NestedMapper();
        return mapper.Nest<TSource, TDestination>();
    }
}