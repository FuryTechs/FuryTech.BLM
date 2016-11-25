using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Data.Common;
using System;
using Effort.Provider;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using BLM.EventListeners;

namespace BLM.EF6.Tests
{
    class FakeEntity
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }

        public bool IsValid { get; set; }

        public bool HasCreatedFlagTriggered { get; set; }
        public bool HasModifiedFlagTriggered { get; set; }
    }

    class MockListener : IEventListener<FakeEntity>
    {
        public MockListener()
        {
            Reset();
        }

        public void Reset()
        {
            OnCreatedTriggered = false;
            OnCreationValidationFailedTriggered = false;
            OnRemovedTriggered = false;
            OnDeletionFailedTriggered = false;
            OnModifiedTriggered = false;
            OnModificationFailedTriggered = false;
        }

        public bool OnCreatedTriggered;
        public void OnCreated(FakeEntity entity, IIdentity user)
        {
            OnCreatedTriggered = true;
        }

        public bool OnCreationValidationFailedTriggered;
        public void OnCreationValidationFailed(FakeEntity entity, IIdentity user)
        {
            OnCreationValidationFailedTriggered = true;
        }

        public bool OnRemovedTriggered;
        public void OnRemoved(FakeEntity entity, IIdentity user)
        {
            OnRemovedTriggered = true;
        }


        public bool OnDeletionFailedTriggered;
        public void OnRemoveFailed(FakeEntity entity, IIdentity user)
        {
            OnDeletionFailedTriggered = true;
        }

        public bool OnModificationFailedTriggered;

        public void OnModificationFailed(FakeEntity originalEntity, FakeEntity modifiedEntity, IIdentity user)
        {
            OnModificationFailedTriggered = true;
        }

        public bool OnModifiedTriggered;
        public void OnModified(FakeEntity originalEntity, FakeEntity modifiedEntity, IIdentity user)
        {
            OnModifiedTriggered = true;
        }

        public FakeEntity OnBeforeCreate(FakeEntity entity, IIdentity user)
        {
            entity.HasCreatedFlagTriggered = true;
            return entity;
        }

        public FakeEntity OnBeforeModify(FakeEntity original, FakeEntity modified, IIdentity user)
        {
            modified.HasModifiedFlagTriggered = true;
            return modified;
        }
    }

    class FakeAuthorizer : BaseEfAuthorizer<FakeEntity>
    {
        public override IQueryable<FakeEntity> AuthorizeCollection(IIdentity usr, IQueryable<FakeEntity> entities)
        {
            Assert.IsNotNull(DbContext);
            return entities;
        }

        public override bool CanRemove(IIdentity usr, FakeEntity entry)
        {
            Assert.IsNotNull(DbContext);
            return entry.IsValid;
        }

        public override bool CanInsert(IIdentity usr, FakeEntity entry)
        {
            Assert.IsNotNull(DbContext);
            return entry.IsValid;
        }

        public override bool CanUpdate(IIdentity usr, FakeEntity original, FakeEntity newEntity)
        {
            Assert.IsNotNull(DbContext);
            return newEntity.IsValid;
        }
    }

    class FakeDbContext : DbContext
    {
        public FakeDbContext(DbConnection connection) : base(connection, true)
        {
        }

        public virtual DbSet<FakeEntity> FakeEntities { get; set; }
    }

    [TestClass]
    public class EntityFrameworkBasicTests
    {
        private EfRepository<FakeEntity> _efRepo;
        private FakeDbContext _db;
        private MockListener _listener;
        private IIdentity _identity;
        private DbConnection _efforConnection;

        [TestInitialize]
        public void Init()
        {
            EffortProviderConfiguration.RegisterProvider();
            _efforConnection = Effort.DbConnectionFactory.CreateTransient();

            _db = new FakeDbContext(_efforConnection);
            _identity = Thread.CurrentPrincipal.Identity;
            _efRepo = new EfRepository<FakeEntity>(_db);
            _listener = EventListenerManager.Current.GetListener<MockListener>() as MockListener;
        }


        [TestCleanup]
        public void Cleanup()
        {
            _efRepo.Dispose();
            _efforConnection?.Dispose();
        }

        [TestMethod]
        public void TestAdd()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            _efRepo.Add(_identity, new FakeEntity { Id = 1, Value = guid, IsValid = true });
            _efRepo.SaveChanges(_identity);
            Assert.IsTrue(_listener.OnCreatedTriggered);
            Assert.IsTrue(_db.FakeEntities.Any());

            var fake = _db.FakeEntities.FirstOrDefault(a => a.Value == guid);
            Assert.IsNotNull(fake);
            Assert.IsTrue(fake.HasCreatedFlagTriggered);

            Assert.AreEqual(guid, _db.FakeEntities.FirstOrDefault().Value);
        }

        [TestMethod]
        public void TestAddRange()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            _efRepo.AddRange(_identity, new List<FakeEntity> { new FakeEntity { Id = 1, Value = guid, IsValid = true } });
            _efRepo.SaveChanges(_identity);
            Assert.IsTrue(_listener.OnCreatedTriggered);
            Assert.IsTrue(_db.FakeEntities.Any());
            Assert.AreEqual(guid, _db.FakeEntities.FirstOrDefault().Value);
        }

        [TestMethod]
        public void TestAddFailed()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            try
            {
                _efRepo.Add(_identity, new FakeEntity { Id = 2, Value = guid, IsValid = false });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(UnauthorizedAccessException));
            }

            try
            {
                _efRepo.SaveChanges(_identity);

            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(UnauthorizedAccessException));
            }

            Assert.IsFalse(_db.FakeEntities.Any(a => a.Value == guid));

            Assert.IsTrue(_listener.OnCreationValidationFailedTriggered);
            Assert.IsFalse(_listener.OnCreatedTriggered);
        }

        [TestMethod]
        public void TestModify()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            var entity = _db.FakeEntities.Add(new FakeEntity { Id = 3, IsValid = true, Value = "alma" });
            _db.SaveChanges();
            Assert.IsNotNull(entity);
            entity.Value = guid;
            _efRepo.SaveChanges(_identity);

            Assert.IsTrue(_listener.OnModifiedTriggered);

            var fake = _db.FakeEntities.FirstOrDefault(a => a.Value == guid);
            Assert.IsNotNull(fake);
            Assert.IsTrue(fake.HasModifiedFlagTriggered);
        }


        [TestMethod]
        public void TestModifyFailed()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            var entity = _db.FakeEntities.Add(new FakeEntity { Id = 3, IsValid = true, Value = "alma" });
            _db.SaveChanges();
            Assert.IsNotNull(entity);
            entity.Value = guid;
            entity.IsValid = false;

            try
            {
                _efRepo.SaveChanges(_identity);
            }
            catch (Exception ex)
            {

                Assert.IsInstanceOfType(ex, typeof(UnauthorizedAccessException));
            }
            Assert.IsFalse(_listener.OnModifiedTriggered);
            Assert.IsTrue(_listener.OnModificationFailedTriggered);
            Assert.IsFalse(_db.FakeEntities.Any(a => a.Value == guid));
        }

        [TestMethod]
        public void TestRemove()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            var entity = _db.FakeEntities.Add(new FakeEntity { Id = 3, IsValid = true, Value = guid });
            _db.SaveChanges();

            _efRepo.Remove(_identity, entity);
            _efRepo.SaveChanges(_identity);

            Assert.IsFalse(_db.FakeEntities.Any(a => a.Value == guid));
            Assert.IsTrue(_listener.OnRemovedTriggered);
        }

        [TestMethod]
        public void TestRemoveRange()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            var entity = _db.FakeEntities.Add(new FakeEntity { Id = 3, IsValid = true, Value = guid });
            _db.SaveChanges();

            _efRepo.RemoveRange(_identity, new List<FakeEntity> { entity });
            _efRepo.SaveChanges(_identity);

            Assert.IsFalse(_db.FakeEntities.Any(a => a.Value == guid));
            Assert.IsTrue(_listener.OnRemovedTriggered);
        }

        [TestMethod]
        public void TestRemoveFailed()
        {
            _listener.Reset();
            var guid = Guid.NewGuid().ToString();
            var entity = _db.FakeEntities.Add(new FakeEntity { Id = 3, IsValid = false, Value = guid });
            _db.SaveChanges();

            try
            {
                _efRepo.Remove(_identity, entity);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(UnauthorizedAccessException));
            }


            _db.FakeEntities.Remove(entity);

            try
            {
                _efRepo.SaveChanges(_identity);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(UnauthorizedAccessException));
            }

            Assert.IsTrue(_db.FakeEntities.Any(a => a.Value == guid));
            Assert.IsFalse(_listener.OnRemovedTriggered);
            Assert.IsTrue(_listener.OnDeletionFailedTriggered);
        }

    }
}
