using FuryTechs.BLM.NetStandard.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FuryTechs.BLM.NetStandard.Tests
{
    public class MockGenericCollectionAuthorizer1<T> : AuthorizeCollection<Entity<T>>
    {
        public override async Task<IQueryable<Entity<T>>> AuthorizeCollectionAsync(IQueryable<Entity<T>> entities, IContextInfo ctx)
        {
            return await Task.Factory.StartNew(() => entities.Where(a => a.IsVisible));
        }
    }

    public class MockGenericCollectionAuthorizer2<T> : AuthorizeCollection<Entity<T>>
    {
        public override async Task<IQueryable<Entity<T>>> AuthorizeCollectionAsync(IQueryable<Entity<T>> entities, IContextInfo ctx)
        {
            return await Task.Factory.StartNew(() => entities.Where(a => a.IsVisible2));
        }
    }

    public class MockGenericIntAuthorizer1 : MockGenericCollectionAuthorizer1<int>
    {

    }

    public class MockGenericGuidAuthorizer1 : MockGenericCollectionAuthorizer1<Guid>
    {

    }

    public class MockGenericIntAuthorizer2 : MockGenericCollectionAuthorizer2<int>
    {

    }

    public class MockGenericGuidAuthorizer2 : MockGenericCollectionAuthorizer2<Guid>
    {

    }


}