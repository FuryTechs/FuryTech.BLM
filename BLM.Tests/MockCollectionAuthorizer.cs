using System.Linq;
using System.Threading.Tasks;
using BLM.Interfaces.Authorize;

namespace BLM.Tests
{

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class MockCollectionAuthorizer : IAuthorizeCollection<MockEntity, MockEntity>
    {
        public async Task<IQueryable<MockEntity>> AuthorizeCollection(IQueryable<MockEntity> entities, IContextInfo ctx)
        {
            return entities.Where(a => a.IsVisible);
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

}