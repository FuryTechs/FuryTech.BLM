using FuryTech.BLM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Register
    {
        /// <summary>
        /// Add BLMEFCore as a resolvable 
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        public static void AddBLMEFCore<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddScoped(typeof(EfRepository<,>));
        }

        /// <summary>
        /// To resolve EfRepository<EntityType> with one generic, this method will add your <typeparamref name="TDbContext"/> class a `DbContext`.
        /// Use this if you have only one DbContext type in your project.
        /// </summary>
        /// <typeparam name="TDbContext">Database context</typeparam>
        /// <param name="services">Service collection</param>
        public static void AddBLMEFCoreDefaultDbContext<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddBLMEFCore<TDbContext>();
            services.AddScoped(typeof(EfRepository<>));
            services.AddScoped<DbContext, TDbContext>();
        }
    }
}
