using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SimpleChat.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.API.Data
{
    //TODO: REFACTOR IT
    public class NGADbContextFactory : IDesignTimeDbContextFactory<SimpleChatDbContext>
    {
        public SimpleChatDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<SimpleChatDbContext>();
            //TODO: Open commented lines!
            // var connectionString = configuration.GetConnectionString("DefaultConnection").Replace("{HostMachineIpAddress}", GetHostMachineIP.Get());
            // builder.UseSqlServer(connectionString);
            return new SimpleChatDbContext(builder.Options);
        }
    }
}
