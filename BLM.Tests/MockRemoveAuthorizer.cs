namespace BLM.Tests
{
    public class MockRemoveAuthorizer : IAuthorizeRemove<MockEntity>
    {
        public AuthorizationResult CanRemove(MockEntity entity, IContextInfo ctx)
        {
            if (entity.IsValid)
            {
                return AuthorizationResult.Success();
            }
            return AuthorizationResult.Fail("The entity is not valid :( ");
        }
    }
}