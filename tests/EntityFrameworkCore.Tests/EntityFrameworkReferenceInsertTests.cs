﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Exceptions;
using FuryTechs.BLM.NetStandard.Tests;
using Xunit.Abstractions;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public class ReferenceInsertTests : AbstractEfRepositoryTest
    {
        protected MockEntity ValidEntity = new MockEntity()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true
        };

        protected MockEntity ValidEntity2 = new MockEntity()
        {
            Id = 2,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true
        };

        protected MockEntity InvalidEntity = new MockEntity()
        {
            Id = 3,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        public ReferenceInsertTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task AddOneWithoutNesting()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));

            await _repoNested.AddAsync(new MockNestedEntity(), _identity);
            await _repoNested.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task AddOneWithSingleNestingExisting()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {ValidEntity}
            }, _identity);
            await _repoNested.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task AddTwoWithSingleNestingExisting()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {ValidEntity, ValidEntity2}
            }, _identity);
            await _repoNested.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task AddOneInvalidNestingExisting()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {InvalidEntity}
            }, _identity);
            await Assert.ThrowsAsync<AuthorizationFailedException>(() => _repoNested.SaveChangesAsync(_identity));
        }

        [Fact]
        public async Task AddOneValidAndOneInvalidNestingExisting()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {ValidEntity, InvalidEntity}
            }, _identity);
            await Assert.ThrowsAsync<AuthorizationFailedException>(() => _repoNested.SaveChangesAsync(_identity));
        }

        [Fact]
        public async Task AddOneDeepInsert()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {new MockEntity {IsValid = true}}
            }, _identity);
            await _repoNested.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task AddTwoDeepInsert()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {new MockEntity {IsValid = true}, new MockEntity {IsValid = true}}
            }, _identity);
            await _repoNested.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task AddOneInvalidDeepInsert()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            var _db = (FakeDbContext) _serviceProvider.GetService(typeof(FakeDbContext));
            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity> {new MockEntity {IsValid = true}, new MockEntity {IsValid = false}}
            }, _identity);

            await Assert.ThrowsAsync<AuthorizationFailedException>(() => _repoNested.SaveChangesAsync(_identity));
            Assert.Equal(0, _db.MockEntities.Count());
            Assert.Equal(0, _db.MockNestedEntities.Count());
        }

        [Fact]
        public async Task ModifyOneDeepInsertTest()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            var _repo = (EfRepository<MockEntity, FakeDbContext>) _serviceProvider.GetService(
                typeof(IRepository<MockEntity, FakeDbContext>));

            await _repoNested.AddAsync(new MockNestedEntity()
            {
                Id = 1,
                MockEntities = new List<MockEntity>
                {
                    new MockEntity
                    {
                        IsValid = true,
                        Id = 3,
                        IsVisible = true,
                        IsVisible2 = true
                    }
                }
            }, _identity);
            await _repoNested.SaveChangesAsync(_identity);

            var first = _repo.Entities(_identity).First();

            first.Guid = Guid.NewGuid().ToString();
            await _repoNested.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task AddOneValidAndOneInvalidDeepInsert()
        {
            var _repoNested =
                (EfRepository<MockNestedEntity, FakeDbContext>) _serviceProvider.GetService(
                    typeof(IRepository<MockNestedEntity, FakeDbContext>));
            var _db = (FakeDbContext) _serviceProvider.GetService(typeof(FakeDbContext));

            await _repoNested.AddAsync(new MockNestedEntity()
            {
                MockEntities = new List<MockEntity>
                {
                    new MockEntity {IsValid = true}, new MockEntity {IsValid = false}
                }
            }, _identity);

            await Assert.ThrowsAsync<AuthorizationFailedException>(() => _repoNested.SaveChangesAsync(_identity));
            Assert.Equal(0, _db.MockEntities.Count());
            Assert.Equal(0, _db.MockNestedEntities.Count());
        }
    }
}
