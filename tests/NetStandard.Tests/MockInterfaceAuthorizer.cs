using System.Linq;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;

namespace FuryTechs.BLM.NetStandard.Tests
{
  public class MockInterfaceAuthorizer : AuthorizeCRUD<IMockEntity>
  {
    public override async Task<IQueryable<IMockEntity>> AuthorizeCollectionAsync(IQueryable<IMockEntity> entities,
      IContextInfo ctx)
    {
      return await Task.FromResult(entities.Where(a => a.IsVisible));
    }

    public override async Task<AuthorizationResult> CanCreateAsync(IMockEntity entity, IContextInfo ctx)
    {
      return await Task.FromResult(entity.IsValid
        ? AuthorizationResult.Success()
        : AuthorizationResult.Fail("Not valid!", entity));
    }

    public override async Task<AuthorizationResult> CanModifyAsync(IMockEntity originalEntity,
      IMockEntity modifiedEntity, IContextInfo ctx)
    {
      return await Task.FromResult(modifiedEntity.IsValid
        ? AuthorizationResult.Success()
        : AuthorizationResult.Fail("Not valid!", modifiedEntity));
    }

    public override async Task<AuthorizationResult> CanRemoveAsync(IMockEntity entity, IContextInfo ctx)
    {
      return await Task.FromResult(entity.IsValid
        ? AuthorizationResult.Success()
        : AuthorizationResult.Fail("Not valid!", entity));
    }
  }
}