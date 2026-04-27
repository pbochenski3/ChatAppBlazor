using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Domain.Repository.Decorators;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Decorators;
using ChatApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure.Providers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEncryptionService, AesEncryptionService>();
            services.AddScoped<MessageRepository>();

            services.AddScoped<IMessageRepository>(sp =>
            {
                var sqlRepo = sp.GetRequiredService<MessageRepository>();
                var encryption = sp.GetRequiredService<IEncryptionService>();

                return new EncryptedMessageRepositoryDecorator(sqlRepo, encryption);
            });

            return services;
        }
    }
}
