using AutoMapper;

namespace Toolkit.Configurations
{
    public sealed class GenericMapper
    {
        public GenericMapper(MapperConfiguration config = null)
        {
            _Config = config;
        }

        private readonly MapperConfiguration _Config;

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var config = _Config ?? new MapperConfiguration(cfg =>
                 cfg.CreateMap<TSource, TDestination>().ReverseMap());
            Mapper mapper = new Mapper(config);
            return mapper.Map<TDestination>(source);
        }

        public List<TDestination> MapList<TSource, TDestination>(List<TSource> sourceList)
        {
            var convertedList = new List<TDestination>();
            sourceList.ForEach(item => { convertedList.Add(Map<TSource, TDestination>(item)); });
            return convertedList;
        }
    }
}