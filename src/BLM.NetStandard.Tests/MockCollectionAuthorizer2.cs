using System.Linq;
using System.Threading.Tasks;
using FuryTech.BLM.NetStandard.Interfaces;

namespace FuryTech.BLM.NetStandard.Tests
{
    public class MockCollectionAuthorizer2 : AuthorizeCollection<MockEntity>
    {
        public override async Task<IQueryable<MockEntity>> AuthorizeCollectionAsync(IQueryable<MockEntity> entities, IContextInfo ctx)
        {
            return await Task.Factory.StartNew(() => entities.Where(a => a.IsVisible2));
        }
    }
}