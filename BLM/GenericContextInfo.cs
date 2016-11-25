using System;
using System.Linq;
using System.Security.Principal;


namespace BLM
{
    public class GenericContextInfo : IContextInfo
    {
        public GenericContextInfo(IIdentity identity)
        {
            Identity = identity;
        }

        public IIdentity Identity { get; }
        public IQueryable<T> GetFullEntitySet<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAuthorizedEntitySet<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
