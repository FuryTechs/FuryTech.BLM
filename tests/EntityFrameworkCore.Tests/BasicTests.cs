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
    protected MockEntity ValidEntity = new()
    {
      Id = 1,
      IsValid = true,
      IsVisible = true,
      IsVisible2 = true
    };

    protected MockEntity InvalidEntity = new()
    {
      Id = 2,
      IsValid = false,
      IsVisible = true,
      IsVisible2 = false
    };

    protected MockEntity InvisibleEntity = new()
    {
      Id = 3,
      IsValid = true,
      IsVisible = false,
      IsVisible2 = true
    };

    protected MockEntity InvisibleEntity2 = new()
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
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
      await repo.AddAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);
      Assert.Equal(1, db.MockEntities.Count());
    }

    [Fact]
    public virtual async Task AddWithoutIdentity()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
      await repo.AddAsync(ValidEntity);
      await repo.SaveChangesAsync();
      Assert.Equal(1, db.MockEntities.Count());
    }

    [Fact]
    public virtual async Task AddFailure()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
      await repo.AddAsync(InvalidEntity, _identity);
      await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await repo.SaveChangesAsync(_identity));
      Assert.Equal(0, db.MockEntities.Count());
    }

    [Fact]
    public virtual async Task AddFailureWithoutIdentity()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
      await repo.AddAsync(InvalidEntity);
      await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await repo.SaveChangesAsync());
      Assert.Equal(0, db.MockEntities.Count());
    }

    [Fact]
    public virtual async Task AddRangeFail()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
      await repo.AddRangeAsync(new List<MockEntity>() { ValidEntity, InvisibleEntity, InvalidEntity }, _identity);
      await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await repo.SaveChangesAsync(_identity));
      Assert.Equal(0, db.MockEntities.Count());
    }

    [Fact]
    public virtual async Task Modify()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      ValidEntity.Guid = Guid.NewGuid().ToString();
      await repo.AddAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);

      var newGuid = Guid.NewGuid().ToString();
      var loadedValid = repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
      loadedValid.Guid = newGuid;
      await repo.SaveChangesAsync(_identity);

      var reloadedValid = repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
      Assert.Equal(reloadedValid.Guid, newGuid);
    }

    [Fact]
    // [ExpectedException(typeof(AuthorizationFailedException))]
    public virtual async Task ModifyFailedAsync()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      await repo.AddAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);

      var validToChange = repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
      validToChange.IsValid = false;

      await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await repo.SaveChangesAsync(_identity));
    }

    [Fact]
    public virtual async Task RemoveAsync()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      await repo.AddAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);

      await repo.RemoveAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);
    }

    [Fact]
    public virtual async Task RemoveRange()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));

      await repo.AddAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);

      await repo.RemoveRangeAsync(new List<MockEntity> { ValidEntity }, _identity);
      await repo.SaveChangesAsync(_identity);
    }


    [Fact]
    // [ExpectedException(typeof(AuthorizationFailedException))]
    public virtual async Task RemoveFailed()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));

      db.Set<MockEntity>().Add(InvalidEntity);
      await db.SaveChangesAsync();
      await repo.RemoveAsync(InvalidEntity, _identity);
      await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await repo.SaveChangesAsync(_identity));
    }

    [Fact]
    // [ExpectedException(typeof(AuthorizationFailedException))]
    public virtual async Task RemoveRangeFailed()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));

      db.Set<MockEntity>().Add(InvalidEntity);
      await db.SaveChangesAsync();

      await repo.RemoveRangeAsync(new List<MockEntity> { InvalidEntity }, _identity);
      await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await repo.SaveChangesAsync(_identity));
    }


    [Fact]
    public virtual async Task AuthorizeCollection()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));

      await repo.AddRangeAsync(new List<MockEntity> { ValidEntity, InvisibleEntity, InvisibleEntity2 },
        _identity);
      await repo.SaveChangesAsync(_identity);
      var authorizationResult = (await repo.EntitiesAsync(_identity));
      var queryResult = authorizationResult.All(a => a.IsVisible && a.IsVisible2);
      Assert.True(queryResult);
    }


    [Fact]
    public virtual async Task CtxAuthorizedEntitySet()
    {
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
      var ctx = new EfContextInfo<MockEntity>(_identity, db, _serviceProvider);

      db.Set<MockEntity>().AddRange(new List<MockEntity>()
        { ValidEntity, InvalidEntity, InvisibleEntity, InvisibleEntity2 });
      await db.SaveChangesAsync();

      Assert.True((await ctx.GetAuthorizedEntitySetAsync<MockEntity>()).All(a => a.IsVisible && a.IsVisible2));
    }

    [Fact]
    public virtual async Task CtxFullEntitySet()
    {
      var db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));

      var ctx = new EfContextInfo<MockEntity>(_identity, db, _serviceProvider);

      db.Set<MockEntity>().AddRange(new List<MockEntity>()
        { ValidEntity, InvalidEntity, InvisibleEntity, InvisibleEntity2 });
      await db.SaveChangesAsync();

      Assert.True(ctx.GetFullEntitySet<MockEntity>().Any(a => !a.IsVisible || !a.IsVisible2));
    }

    [Fact]
    public virtual async Task GetSetEntityState()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));

      await repo.AddAsync(ValidEntity, _identity);
      Assert.Equal(EntityState.Added, repo.GetEntityState(ValidEntity));
      repo.SetEntityState(ValidEntity, EntityState.Modified);
      Assert.Equal(EntityState.Modified, repo.GetEntityState(ValidEntity));
    }

    [Fact]
    public virtual async Task SkipUnchangedEntities()
    {
      var repo = (EfRepository<MockEntity>)_serviceProvider.GetService(
        typeof(IRepository<MockEntity, FakeDbContext>));

      await repo.AddAsync(ValidEntity, _identity);
      await repo.SaveChangesAsync(_identity);

      var loaded = repo.Entities(_identity).FirstOrDefault(a => a.Id == ValidEntity.Id);
      ValidEntity.Guid = Guid.NewGuid().ToString();
      repo.SetEntityState(loaded, EntityState.Unchanged);

      await repo.SaveChangesAsync(_identity);
    }
  }
}