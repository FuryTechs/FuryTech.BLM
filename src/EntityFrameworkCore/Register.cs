using FuryTechs.BLM.EntityFrameworkCore;
using FuryTechs.BLM.EntityFrameworkCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains the extension methods to add Blm EfCore to IServiceCollection
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// Add BLMEFCore as a resolvable 
        /// </summary>
        /// <param name="services"></param>
        public static void AddBlmEfCore(this IServiceCollection services)
        {
            services.AddScoped(typeof(EfRepository<,>));
        }

        /// <summary>
        /// To resolve EfRepository<![CDATA[<EntityType>]]> with one generic, this method will add your <typeparamref name="TDbContext"/> class a `DbContext`.
        /// Use this if you have only one DbContext type in your project.
        /// </summary>
        /// <typeparam name="TDbContext">Database context</typeparam>
        /// <param name="services">Service collection</param>
        /// <param name="identityResolver">(optional) Identity resolver implementation. If null, you have to provide identity everywhere in the code, or register it for yourself</param>
        public static void AddBlmEfCoreDefaultDbContext<TDbContext>(this IServiceCollection services, IIdentityResolver identityResolver = null)
            where TDbContext : DbContext
        {
            services.AddBlmEfCore();
            services.AddScoped(typeof(EfRepository<>));
            services.AddScoped<DbContext, TDbContext>();

            if (identityResolver != null)
            {
                services.AddScoped<IIdentityResolver>((p) => identityResolver);
            }
        }

    }
}
