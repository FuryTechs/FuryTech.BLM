using System;
using System.Collections.Generic;
using System.Linq;
using FuryTechs.BLM.NetStandard.Tests;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Exceptions;

using Xunit;
using Xunit.Abstractions;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public class BasicTests : AbstractEfRepositoryTest
    {
        protected MockEntity ValidEntity = new MockEntity()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true

        };

        protected MockEntity InvalidEntity = new MockEntity()
        {
            Id = 2,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        protected MockEntity InvisibleEntity = new MockEntity()
        {
            Id = 3,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        protected MockEntity InvisibleEntity2 = new MockEntity()
        {
            Id = 4,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        public BasicTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public virtual async Task Add()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
            await _repo.AddAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);
            Assert.Equal(1, _db.MockEntities.Count());
        }

        [Fact]
        public virtual async Task AddFailure()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
            await _repo.AddAsync(_identity, InvalidEntity);
            await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _repo.SaveChangesAsync(_identity));
            Assert.Equal(0, _db.MockEntities.Count());
        }

        [Fact]
        public virtual async Task AddRangeFail()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
            await _repo.AddRangeAsync(_identity, new List<MockEntity>() { ValidEntity, InvisibleEntity, InvalidEntity });
            await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _repo.SaveChangesAsync(_identity));
            Assert.Equal(0, _db.MockEntities.Count());
        }

        [Fact]
        public virtual async Task Modify()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            ValidEntity.Guid = Guid.NewGuid().ToString();
            await _repo.AddAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);

            var newGuid = Guid.NewGuid().ToString();
            var loadedValid = _repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
            loadedValid.Guid = newGuid;
            await _repo.SaveChangesAsync(_identity);

            var reloadedValid = _repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
            Assert.Equal(reloadedValid.Guid, newGuid);
        }

        [Fact]
        // [ExpectedException(typeof(AuthorizationFailedException))]
        public virtual async Task ModifyFailedAsync()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            await _repo.AddAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);

            var validToChange = _repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
            validToChange.IsValid = false;

            await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _repo.SaveChangesAsync(_identity));
        }

        [Fact]
        public virtual async Task RemoveAsync()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            await _repo.AddAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);

            await _repo.RemoveAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);
        }

        [Fact]
        public virtual async Task RemoveRange()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _repo.AddAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);

            await _repo.RemoveRangeAsync(_identity, new List<MockEntity> { ValidEntity });
            await _repo.SaveChangesAsync(_identity);
        }



        [Fact]
        // [ExpectedException(typeof(AuthorizationFailedException))]
        public virtual async Task RemoveFailed()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));

            _db.Set<MockEntity>().Add(InvalidEntity);
            await _db.SaveChangesAsync();
            await _repo.RemoveAsync(_identity, InvalidEntity);
            await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _repo.SaveChangesAsync(_identity));
        }

        [Fact]
        // [ExpectedException(typeof(AuthorizationFailedException))]
        public virtual async Task RemoveRangeFailed()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));

            _db.Set<MockEntity>().Add(InvalidEntity);
            await _db.SaveChangesAsync();

            await _repo.RemoveRangeAsync(_identity, new List<MockEntity> { InvalidEntity });
            await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _repo.SaveChangesAsync(_identity));
        }


        [Fact]
        public virtual async Task AuthorizeCollection()
        {
            try
            {
                var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

                await _repo.AddRangeAsync(_identity, new List<MockEntity>() { ValidEntity, InvisibleEntity, InvisibleEntity2 });
                await _repo.SaveChangesAsync(_identity);
                var authorizationResult = (await _repo.EntitiesAsync(_identity));
                var queryResult = authorizationResult.All(a => a.IsVisible && a.IsVisible2);
                Assert.True(queryResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Fact]
        public virtual async Task CtxAuthorizedEntitySet()
        {
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
            var ctx = new EfContextInfo(_identity, _db, _serviceProvider);

            _db.Set<MockEntity>().AddRange(new List<MockEntity>() { ValidEntity, InvalidEntity, InvisibleEntity, InvisibleEntity2 });
            await _db.SaveChangesAsync();

            Assert.True((await ctx.GetAuthorizedEntitySetAsync<MockEntity>()).All(a => a.IsVisible && a.IsVisible2));
        }

        [Fact]
        public virtual async Task CtxFullEntitySet()
        {
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));

            var ctx = new EfContextInfo(_identity, _db, _serviceProvider);

            _db.Set<MockEntity>().AddRange(new List<MockEntity>() { ValidEntity, InvalidEntity, InvisibleEntity, InvisibleEntity2 });
            await _db.SaveChangesAsync();

            Assert.True(ctx.GetFullEntitySet<MockEntity>().Any(a => !a.IsVisible || !a.IsVisible2));
        }

        [Fact]
        public virtual async Task GetSetEntityState()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _repo.AddAsync(_identity, ValidEntity);
            Assert.Equal(EntityState.Added, _repo.GetEntityState(ValidEntity));
            _repo.SetEntityState(ValidEntity, EntityState.Modified);
            Assert.Equal(EntityState.Modified, _repo.GetEntityState(ValidEntity));
        }

        [Fact]
        public virtual async Task SkipUnchangedEntities()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _repo.AddAsync(_identity, ValidEntity);
            await _repo.SaveChangesAsync(_identity);

            var loaded = _repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
            ValidEntity.Guid = Guid.NewGuid().ToString();
            _repo.SetEntityState(loaded, EntityState.Unchanged);

            await _repo.SaveChangesAsync(_identity);
        }


    }
}
