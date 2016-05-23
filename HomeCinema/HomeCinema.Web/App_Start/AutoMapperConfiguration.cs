using AutoMapper;
using HomeCinema.Web.Infrastructure.Mappings;

namespace HomeCinema.Web
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToViewModelMappingProfile>();
            });
        }
    }
}