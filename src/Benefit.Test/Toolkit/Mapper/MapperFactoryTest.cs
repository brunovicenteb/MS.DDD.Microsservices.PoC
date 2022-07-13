using Toolkit.Interfaces;
using Toolkit.Mapper;

namespace Benefit.Test.Toolkit.Mapper
{
    public class MapperFactoryTest
    {
        private class SourceClass<T>
        {
            public T value { get; set; }
        }

        private class DestinationClass<T>
        {
            public T value { get; set; }
        }

        [Fact]
        public void TestMapperFactorWithNestedClass()
        {
            //arrange
            IGenericMapper genericMapping = MapperFactory.Nest<SourceClass<int>, DestinationClass<string>>()
                .Build<DestinationClass<string>, SourceClass<int>>();

            DestinationClass<string> letterP = new DestinationClass<string> { value = "P" };
            SourceClass<int> numberOne = new SourceClass<int> { value = 1 };

            //act
            var numberConverted = genericMapping.Map<SourceClass<int>, DestinationClass<string>>(numberOne);

            Action convertionError = () => { genericMapping.Map<DestinationClass<string>, SourceClass<int>>(letterP); };
            var exception = Record.Exception(convertionError);

            //assert
            Assert.IsType<DestinationClass<string>>(numberConverted);
            Assert.NotNull(exception);
            Assert.IsType<AutoMapper.AutoMapperMappingException>(exception);
        }


        [Fact]
        public void TestMapperFactorWithoutNestedClass()
        {
            //arrange
            IGenericMapper genericMapping = MapperFactory.Map<SourceClass<int>, DestinationClass<string>>();
            SourceClass<int> numberTen = new SourceClass<int> { value = 10 };

            //act
            var numberConverted = genericMapping.Map<SourceClass<int>, DestinationClass<string>>(numberTen);

            //assert
            Assert.IsType<DestinationClass<string>>(numberConverted);
        }
    }
}
