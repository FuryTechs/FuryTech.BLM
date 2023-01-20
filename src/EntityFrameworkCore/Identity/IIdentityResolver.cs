using System.Security.Principal;

namespace FuryTechs.BLM.EntityFrameworkCore.Identity
{
  /// <summary>
  /// Interface which can resolve the User's Identity
  /// </summary>
  public interface IIdentityResolver
  {
    /// <summary>
    /// Gets the user's identity
    /// </summary>
    /// <returns>User's identity</returns>
    IIdentity GetIdentity();
  }
}