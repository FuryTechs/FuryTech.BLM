using System.Linq;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;

namespace FuryTechs.BLM.NetStandard.Tests.Generic
{
    public class MockCollectionAuthorizerIGeneric<T> : AuthorizeCollection<IGenericEntity<T>>
    {
        public override async Task<IQueryable<IGenericEntity<T>>> AuthorizeCollectionAsync(IQueryable<IGenericEntity<T>> entities, IContextInfo ctx)
        {
            return await Task.Factory.StartNew(() => entities.Where(a => a.IsVisible2));
        }

    }

}