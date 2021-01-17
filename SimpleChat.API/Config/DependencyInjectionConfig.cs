using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleChat.Core.Auth;
using SimpleChat.Core.Helper;
using SimpleChat.Data;
using SimpleChat.Data.Service;
using SimpleChat.Data.SubStructure;
using StackExchange.Redis;

namespace SimpleChat.API.Config
{
    internal static class DependencyInjectionConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration Configuration, ref IMapper mapper)
        {
            services.AddSingleton(mapper);
            services.AddDbContext<SimpleChatDbContext>(db =>
                db.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("SimpleChat.Data")));
            services.AddScoped<UnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IBaseService<,,,>), typeof(BaseService<,,,>));

            services.AddTransient<IChatRoomService, ChatRoomService>();
            services.AddTransient<IChatRoomUserService, ChatRoomUserService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IUserService, UserService>();

            services.AddSingleton<IConfiguration>(provider => Configuration);
            services.AddTransient<ITokenService, TokenService>();

            services.AddSingleton<RedisDbContext>();
            services.AddTransient(typeof(RedisClient<,>));

            services.AddTransient(typeof(APIResult));
            services.AddTransient(typeof(APIResult<>));


            // services.AddTransient<IConnectionMultiplexer>();

            // Assembly.GetAssembly(typeof(SimpleChatDbContext))
            //     .GetTypes()
            //     .Where(item => item.GetInterfaces()
            //     .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof()
            //         && !item.IsAbstract
            //         && !item.IsInterface
            //         && item.Name.ToUpper() != "BASESERVICE")
            //     .ToList()
            //     .ForEach(assignedTypes =>
            //     {
            //         var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IBaseService<,,,>));
            //         services.AddScoped(serviceType, assignedTypes);
            //     });
        }
    }
}
