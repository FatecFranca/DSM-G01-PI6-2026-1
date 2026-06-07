using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Sleep.Application.Services.AutoMapper;

namespace CommonTestUtilities.Mapper
{
    public class MappingBuilder
    {
        public static IMapper Build()
        {
            return new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping());
            }, NullLoggerFactory.Instance).CreateMapper();
        }
    }
}
