using AutoMapper;
using Benefit.API.DTO.Beneficiary;
using Benefit.Domain.AggregatesModel.BeneficiaryAggregate;

namespace Benefit.API.Mapping
{
    public class MappingConfig : Profile
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<BeneficiaryOutput, Beneficiary>();
                config.CreateMap<Beneficiary, BeneficiaryOutput>();
                config.CreateMap<BeneficiaryInput, Beneficiary>();
                config.CreateMap<Beneficiary, BeneficiaryInput>();
            });

            return mappingConfig;
        }
    }
}
