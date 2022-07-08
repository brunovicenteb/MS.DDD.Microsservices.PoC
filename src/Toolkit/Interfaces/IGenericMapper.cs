namespace Toolkit.Interfaces;

public interface INestedMapper
{
    INestedMapper Nest<TSource, TDestination>();

    IGenericMapper Build<TSource, TDestination>();
}

public interface IGenericMapper
{
    TDestination Map<TSource, TDestination>(TSource source);
}