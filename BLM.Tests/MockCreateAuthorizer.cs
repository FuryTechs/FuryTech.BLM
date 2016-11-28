namespace BLM.Tests
{
    public class MockCreateAuthorizer : IAuthorizeCreate<MockEntity> {
        public AuthorizationResult CanCreate(MockEntity entity, IContextInfo ctx)
        {
            if (entity.IsValid)
            {
                return AuthorizationResult.Success();
            }
            return AuthorizationResult.Fail("The entity is not valid :( ");
        }
    }
}