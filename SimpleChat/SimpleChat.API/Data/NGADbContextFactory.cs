using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NGA.Data;
using NGA.MonolithAPI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NGA.MonolithAPI.Data
{
    public class NGADbContextFactory : IDesignTimeDbContextFactory<NGADbContext>
    {
        public NGADbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<NGADbContext>();
            //TODO: Open commented lines!
            // var connectionString = configuration.GetConnectionString("DefaultConnection").Replace("{HostMachineIpAddress}", GetHostMachineIP.Get());
            // builder.UseSqlServer(connectionString);
            return new NGADbContext(builder.Options);
        }
    }
}
