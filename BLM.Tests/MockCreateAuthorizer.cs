using BLM.Interfaces.Authorize;
using System.Threading.Tasks;
using System;

namespace BLM.Tests
{
    public class MockCreateAuthorizer : IAuthorizeCreate<MockEntity> {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<AuthorizationResult> CanCreateAsync(MockEntity entity, IContextInfo ctx)
        {
            if (entity.IsValid)
            {
                return AuthorizationResult.Success();
            }
            return AuthorizationResult.Fail("The entity is not valid :( ", entity);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }

}