using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard;
using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuryTechs.BLM.EntityFrameworkCore
{
    public class EfContextInfo : IContextInfo
    {

        private readonly DbContext _dbcontext;
        private readonly IServiceProvider _serviceProvider;

        public EfContextInfo(IIdentity identity, DbContext ctx, IServiceProvider serviceProvider)
        {
            _dbcontext = ctx;
            Identity = identity;
            _serviceProvider = serviceProvider;
        }

        public IIdentity Identity { get; }
        public IQueryable<T> GetFullEntitySet<T>() where T : class
        {
            return _dbcontext.Set<T>();
        }

        public IQueryable<T> GetAuthorizedEntitySet<T>() where T : class
        {
            return Authorize.Collection(_dbcontext.Set<T>(), new EfContextInfo(Identity, _dbcontext, _serviceProvider), _serviceProvider);
        }

        public async Task<IQueryable<T>> GetAuthorizedEntitySetAsync<T>() where T : class
        {
            return await Authorize.CollectionAsync(_dbcontext.Set<T>(), new EfContextInfo(Identity, _dbcontext, _serviceProvider), _serviceProvider);
        }
    }
}
