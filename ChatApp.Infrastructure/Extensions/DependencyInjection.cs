using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IMessageRepository, MessageRepository>();
            return services;
        }
    }
}
