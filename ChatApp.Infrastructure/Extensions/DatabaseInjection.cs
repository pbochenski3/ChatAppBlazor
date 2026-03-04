using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Extensions
{
    public static class DatabaseInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
