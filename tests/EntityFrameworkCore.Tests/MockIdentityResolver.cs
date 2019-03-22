using FuryTechs.BLM.EntityFrameworkCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    class MockIdentityResolver : IIdentityResolver
    {
        public IIdentity GetIdentity()
        {
            return new GenericIdentity("gallayb");
        }
    }
}
