namespace BLM.Tests
{
    public class MockModifyAuthorizer : IAuthorizeModify<MockEntity> {
        public AuthorizationResult CanModify(MockEntity originalEntity, MockEntity modifiedEntity, IContextInfo ctx)
        {
            if (modifiedEntity.IsValid)
            {
                return AuthorizationResult.Success();
            }
            return AuthorizationResult.Fail("The entity is not valid :( ", modifiedEntity);
        }
    }
}