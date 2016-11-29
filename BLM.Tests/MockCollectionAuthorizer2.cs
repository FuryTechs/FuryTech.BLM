using System.Linq;
using BLM.Interfaces.Authorize;
using System.Threading.Tasks;

namespace BLM.Tests
{
    public class MockCollectionAuthorizer2 : IAuthorizeCollection<MockEntity>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IQueryable<MockEntity>> AuthorizeCollectionAsync(IQueryable<MockEntity> entities, IContextInfo ctx)
        {
            return entities.Where(a => a.IsVisible2);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}