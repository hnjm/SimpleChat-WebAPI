using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleChat.Data;

namespace SimpleChat.API.Config
{
    internal static class AutoMapperConfig
    {
        internal static IMapper Add(ref IServiceCollection services, IConfiguration Configuration)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddAutoMapper(typeof(Startup).Assembly);

            return mapper;
        }
    }
}
