using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using BLM.Tests;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BLM.EF6.Tests
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbConnection connection) : base(connection, true)
        {

        }

        public virtual DbSet<MockEntity> MockEntities { get; set; }
    }

    [TestClass]
    public class EntityFrameworkBasicTests
    {
        private FakeDbContext _db;
        private EfRepository<MockEntity> _repo;
        private IIdentity _identity;

        private MockEntity valid = new MockEntity()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true

        };

        private MockEntity invalid = new MockEntity()
        {
            Id = 2,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        private MockEntity invisible = new MockEntity()
        {
            Id = 3,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        private MockEntity invisible2 = new MockEntity()
        {
            Id = 4,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        IContextInfo ctx = new GenericContextInfo(Thread.CurrentPrincipal.Identity);

        [TestInitialize]
        public void Init()
        {
            EffortProviderConfiguration.RegisterProvider();
            var efforConnection = Effort.DbConnectionFactory.CreateTransient();
            _db = new FakeDbContext(efforConnection);
            _repo = new EfRepository<MockEntity>(_db);
            _identity = Thread.CurrentPrincipal.Identity;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repo?.Dispose();
            _db?.Dispose();
        }

        [TestMethod]
        public async Task Add()
        {
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public async Task AddFailure()
        {
            await _repo.AddAsync(_identity, invalid);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public async Task AddFailureOnSave()
        {
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);
            _db.Set<MockEntity>().FirstOrDefault().IsValid = false;

            await _repo.SaveChangesAsync(_identity);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public async Task AddRangeFail()
        {
            await _repo.AddRangeAsync(_identity, new List<MockEntity>() { valid, invisible, invalid });
        }

        [TestMethod]
        public async Task Modify()
        {
            valid.Guid = Guid.NewGuid().ToString();
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);

            var newGuid = Guid.NewGuid().ToString();
            var loadedValid = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            loadedValid.Guid = newGuid;
            await _repo.SaveChangesAsync(_identity);

            var reloadedValid = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            Assert.AreEqual(reloadedValid.Guid, newGuid);
        }
        
        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public async Task ModifyFailedAsync()
        {
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);

            var validToChange = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            validToChange.IsValid = false;

            await _repo.SaveChangesAsync(_identity);
        }

        [TestMethod]
        public async Task RemoveAsync()
        {
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);

            await _repo.RemoveAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);
        }

        [TestMethod]
        public async Task RemoveRange()
        {
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);

            await _repo.RemoveRangeAsync(_identity, new List<MockEntity> { valid });
            await _repo.SaveChangesAsync(_identity);
        }



        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public async Task RemoveFailed()
        {
            _db.Set<MockEntity>().Add(invalid);
            await _db.SaveChangesAsync();

            await _repo.RemoveAsync(_identity, invalid);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public async Task RemoveRangeFailed()
        {
            _db.Set<MockEntity>().Add(invalid);
            await _db.SaveChangesAsync();

            await _repo.RemoveRangeAsync(_identity, new List<MockEntity> { invalid });

        }


        [TestMethod]
        public async Task AuthorizeCollection()
        {
            await _repo.AddRangeAsync(_identity, new List<MockEntity>() { valid, invisible, invisible2 });
            await _repo.SaveChangesAsync(_identity);

            Assert.IsTrue(_repo.Entities(_identity).All(a => a.IsVisible && a.IsVisible2));
        }


        [TestMethod]
        public async Task CtxAuthorizedEntitySet()
        {
            var ctx = new EfContextInfo(_identity, _db);

            _db.Set<MockEntity>().AddRange(new List<MockEntity>() { valid, invalid, invisible, invisible2 });
            await _db.SaveChangesAsync();

            Assert.IsTrue((await ctx.GetAuthorizedEntitySetAsync<MockEntity>()).All(a => a.IsVisible && a.IsVisible2));
        }

        [TestMethod]
        public async Task CtxFullEntitySet()
        {
            var ctx = new EfContextInfo(_identity, _db);

            _db.Set<MockEntity>().AddRange(new List<MockEntity>() { valid, invalid, invisible, invisible2 });
            await _db.SaveChangesAsync();

            Assert.IsTrue(ctx.GetFullEntitySet<MockEntity>().Any(a => !a.IsVisible || !a.IsVisible2));
        }

        [TestMethod]
        public async Task GetSetEntityState()
        {
            await _repo.AddAsync(_identity, valid);
            Assert.AreEqual(_repo.GetEntityState(valid), EntityState.Added);
            _repo.SetEntityState(valid, EntityState.Modified);
            Assert.AreEqual(_repo.GetEntityState(valid), EntityState.Modified);
        }

        [TestMethod]
        public async Task SkipUnchangedEntities()
        {
            await _repo.AddAsync(_identity, valid);
            await _repo.SaveChangesAsync(_identity);

            var loaded = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            valid.Guid = Guid.NewGuid().ToString();
            _repo.SetEntityState(loaded, EntityState.Unchanged);

            await _repo.SaveChangesAsync(_identity);
        }
    }
}
