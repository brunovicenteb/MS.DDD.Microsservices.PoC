using System.Runtime.CompilerServices;
using Toolkit.Interfaces;

namespace Toolkit.Mapper;

internal class GenericMapper<TS, TD> : IGenericMapper
{
    private AutoMapper.Mapper _Mapper;

    public GenericMapper()
    {
        var cfg = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TS, TD>();
        });
        _Mapper = new AutoMapper.Mapper(cfg);
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _Mapper.Map<TDestination>(source);
    }
}