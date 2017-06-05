﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLM.NetStandard.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.EF7.Tests
{
    [TestClass]
    public class AbstractEfRepositoryTest
    {
        protected FakeDbContext _db;
        protected EfRepository<MockEntity> _repo;
        protected EfRepository<MockNestedEntity> _repoNested;
        protected EfRepository<MockInterpretedEntity> _repoInterpreted;
        protected IIdentity _identity;

        [TestInitialize]
        public void Init()
        {
            var InMemoryOptions = InMemoryDbContextOptionsExtensions.UseTransientInMemoryDatabase(new DbContextOptionsBuilder(new DbContextOptions<FakeDbContext>()));
            
            _db = new FakeDbContext(InMemoryOptions.Options);

            _repo = new EfRepository<MockEntity>(_db);
            _repoNested = new EfRepository<MockNestedEntity>(_db);
            _repoInterpreted = new EfRepository<MockInterpretedEntity>(_db);
            _identity = new GenericIdentity("gallayb");

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

            Entity3 = new LogicalDeleteEntity()
            {
                Id = 3,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            _repo?.Dispose();
            _db?.Dispose();
        }

        protected MockEntity Entity1 { get; set; }
        protected LogicalDeleteEntity Entity2 { get; set; }
        protected LogicalDeleteEntity Entity3 { get; set; }
    }
}
