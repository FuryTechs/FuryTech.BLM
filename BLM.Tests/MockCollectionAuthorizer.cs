using System.Linq;

namespace BLM.Tests
{
    public class MockCollectionAuthorizer : IAuthorizeCollection<MockEntity>
    {
        public IQueryable<MockEntity> AuthorizeCollection(IQueryable<MockEntity> entities, IContextInfo ctx)
        {
            return entities.Where(a=>a.IsVisible);
        }
    }
}