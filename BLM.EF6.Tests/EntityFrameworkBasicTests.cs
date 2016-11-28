using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using BLM.Tests;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void Add()
        {
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void AddFailure()
        {
            _repo.Add(_identity, invalid);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void AddFailureOnSave()
        {
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);
            _db.Set<MockEntity>().FirstOrDefault().IsValid = false;

            _repo.SaveChanges(_identity);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void AddRangeFail()
        {
            _repo.AddRange(_identity, new List<MockEntity>(){valid, invisible, invalid});
        }

        [TestMethod]
        public void Modify()
        {
            valid.Guid = Guid.NewGuid().ToString();
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);

            var newGuid = Guid.NewGuid().ToString();
            var loadedValid = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            loadedValid.Guid = newGuid;
            _repo.SaveChanges(_identity);

            var reloadedValid = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            Assert.AreEqual(reloadedValid.Guid, newGuid);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void ModifyFailed()
        {
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);

            var validToChange = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            validToChange.IsValid = false;

            _repo.SaveChanges(_identity);
        }

        [TestMethod]
        public void Remove()
        {
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);

            _repo.Remove(_identity, valid);
            _repo.SaveChanges(_identity);
        }

        [TestMethod]
        public void RemoveRange()
        {
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);

            _repo.RemoveRange(_identity, new List<MockEntity>{valid});
            _repo.SaveChanges(_identity);
        }



        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void RemoveFailed()
        {
            _db.Set<MockEntity>().Add(invalid);
            _db.SaveChanges();

            _repo.Remove(_identity, invalid);

        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void RemoveRangeFailed()
        {
            _db.Set<MockEntity>().Add(invalid);
            _db.SaveChanges();

            _repo.RemoveRange(_identity, new List<MockEntity>{invalid});

        }


        [TestMethod]
        public void AuthorizeCollection()
        {
            _repo.AddRange(_identity, new List<MockEntity>(){valid, invisible, invisible2});
            _repo.SaveChanges(_identity);

            Assert.IsTrue(_repo.Entities(_identity).All(a => a.IsVisible && a.IsVisible2));
        }


        [TestMethod]
        public void CtxAuthorizedEntitySet()
        {
            var ctx = new EfContextInfo(_identity, _db);

            _db.Set<MockEntity>().AddRange(new List<MockEntity>() {valid, invalid, invisible, invisible2});
            _db.SaveChanges();

            Assert.IsTrue(ctx.GetAuthorizedEntitySet<MockEntity>().All(a=>a.IsVisible && a.IsVisible2));
        }

        [TestMethod]
        public void CtxFullEntitySet()
        {
            var ctx = new EfContextInfo(_identity, _db);

            _db.Set<MockEntity>().AddRange(new List<MockEntity>() { valid, invalid, invisible, invisible2 });
            _db.SaveChanges();

            Assert.IsTrue(ctx.GetFullEntitySet<MockEntity>().Any(a => !a.IsVisible || !a.IsVisible2));
        }

        [TestMethod]
        public void GetSetEntityState()
        {
            _repo.Add(_identity, valid);
            Assert.AreEqual(_repo.GetEntityState(valid), EntityState.Added);
            _repo.SetEntityState(valid, EntityState.Modified);
            Assert.AreEqual(_repo.GetEntityState(valid), EntityState.Modified);
        }

        [TestMethod]
        public void SkipUnchangedEntities()
        {
            _repo.Add(_identity, valid);
            _repo.SaveChanges(_identity);

            var loaded = _repo.Entities(_identity).FirstOrDefault(a => a.Id == valid.Id);
            valid.Guid = Guid.NewGuid().ToString();
            _repo.SetEntityState(loaded, EntityState.Unchanged);

            _repo.SaveChanges(_identity);
        }
    }
}
