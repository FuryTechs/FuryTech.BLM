using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using BLM.Interfaces.Listen;
using BLM.Tests;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.EF6.Tests
{
    /// <summary>
    /// Summary description for ListenerTests
    /// </summary>
    [TestClass]
    public class EntityFrameworkRepositoryListenerTests : AbstractEfRepositoryTest
    {
        private MockEntity Entity1 { get; set; }
        private LogicalDeleteEntity Entity2 { get; set; }

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            EfChangeListener.Reset();
            Entity1 = new MockEntity()
            {
                Id = 1,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };

            Entity2 = new LogicalDeleteEntity()
            {
                Id = 2,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };
        }

        [TestCleanup]
        public override void Cleanup()
        {
            _repo?.Dispose();
            _db?.Dispose();
        }

        /// <summary>
        /// Adds two entities to the _repo
        /// </summary>
        /// <returns></returns>
        protected async Task _add()
        {
            await _repo.AddAsync(_identity, Entity1);
            await _repo.AddAsync(_identity, Entity2);
            await _repo.SaveChangesAsync(_identity);
        }

        [TestMethod]
        public async Task Add()
        {
            await _add();
            Assert.AreEqual(_repo.Entities(_identity).Count(), 2);
        }

        [TestMethod]
        public async Task EFListenRemove()
        {
            await _add();;
            Assert.AreEqual(2, EfChangeListener.CreatedEntities.Count);

            await _repo.RemoveRangeAsync(_identity, _repo.Entities(_identity));
            await _repo.SaveChangesAsync(_identity);

            Assert.AreEqual(1, _repo.Entities(_identity).Count());
            Assert.AreEqual(1, EfChangeListener.RemovedEntities.Count);
            // The LogicalDeleting mock entity should just modified
            Assert.AreEqual(true, EfChangeListener.WasOnModifiedCalled);


        }
    }

    class EfChangeListener : Listener<MockEntity>
    {
        public static List<MockEntity> CreatedEntities { get; set; }
        public static List<MockEntity> ModifiedOriginalEntities { get; set; }
        public static List<MockEntity> ModifiedNewEntities { get; set; }
        public static List<MockEntity> RemovedEntities { get; set; }

        public new static void Reset()
        {
            CreatedEntities = new List<MockEntity>();
            ModifiedNewEntities = new List<MockEntity>();
            ModifiedOriginalEntities = new List<MockEntity>();
            RemovedEntities = new List<MockEntity>();
            Listener<MockEntity>.Reset();
        }

        public override async Task OnCreatedAsync(MockEntity entity, IContextInfo context)
        {
            await base.OnCreatedAsync(entity, context);
            CreatedEntities.Add(entity);
        }

        public override async Task OnModifiedAsync(MockEntity originalEntity, MockEntity modifiedEntity, IContextInfo context)
        {
            await base.OnModifiedAsync(originalEntity, modifiedEntity, context);
            ModifiedOriginalEntities.Add(originalEntity);
            ModifiedNewEntities.Add(modifiedEntity);
        }

        public override async Task OnRemovedAsync(MockEntity entity, IContextInfo context)
        {
            await base.OnRemovedAsync(entity, context);
            RemovedEntities.Add(entity);
        }
    }
}
