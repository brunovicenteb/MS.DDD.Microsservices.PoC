using AutoMapper;

namespace Toolkit.Configurations
{
    public sealed class GenericMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            Mapper mapper = new Mapper
            (
                new MapperConfiguration
                (
                    config => config.CreateMap<TSource, TDestination>().ReverseMap()
                )
            );
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