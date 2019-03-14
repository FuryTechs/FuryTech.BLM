using System.Linq;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;

namespace FuryTechs.BLM.NetStandard.Tests.Generic
{
    public class MockCollectionAuthorizerGeneric<T> : AuthorizeCollection<GenericEntity<T>>
    {
        public override async Task<IQueryable<GenericEntity<T>>> AuthorizeCollectionAsync(IQueryable<GenericEntity<T>> entities, IContextInfo ctx)
        {
            return await Task.Factory.StartNew(() => entities.Where(a => a.IsVisible));
        }

    }

}