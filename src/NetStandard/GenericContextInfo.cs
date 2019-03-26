using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;

namespace FuryTechs.BLM.NetStandard
{
    public class GenericContextInfo : IContextInfo
    {
        public GenericContextInfo(IIdentity identity)
        {
            Identity = identity;
        }

        public IIdentity Identity { get; }

        public Type EntityType => throw new NotImplementedException();

        public IQueryable<T> GetFullEntitySet<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> GetAuthorizedEntitySetAsync<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
