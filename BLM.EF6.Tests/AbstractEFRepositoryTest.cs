using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLM.Tests;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.EF6.Tests
{
    public abstract class AbstractEfRepositoryTest
    {
        protected FakeDbContext _db;
        protected EfRepository<MockEntity> _repo;
        protected IIdentity _identity;

        [TestInitialize]
        public virtual void Init()
        {
            EffortProviderConfiguration.RegisterProvider();
            var efforConnection = Effort.DbConnectionFactory.CreateTransient();
            _db = new FakeDbContext(efforConnection);
            _repo = new EfRepository<MockEntity>(_db);
            _identity = Thread.CurrentPrincipal.Identity;
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            _repo?.Dispose();
            _db?.Dispose();
        }
    }
}
