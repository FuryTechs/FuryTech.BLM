using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

using FuryTechs.BLM.NetStandard.Tests;
using FuryTechs.BLM.NetStandard.Exceptions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    /// <summary>
    /// Summary description for ListenerTests
    /// </summary>

    public class RepositoryListenerTests : AbstractEfRepositoryTest
    {

        public RepositoryListenerTests(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// Adds two entities to the _repo
        /// </summary>
        /// <returns></returns>
        protected async Task _add(EfRepository<MockEntity> _repo)
        {
            //var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _repo.AddAsync(_identity, Entity1);
            await _repo.AddAsync(_identity, Entity2);
            await _repo.SaveChangesAsync(_identity);
        }

        [Fact]
        public async Task Add()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            await _add(_repo);
            Assert.Equal(2, _repo.Entities(_identity).Count());
        }

        [Fact]
        public async Task LogicalDelete_ALL_Throw_SecurityRiskException()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _add(_repo);

            Assert.Equal(2, EfChangeListener.CreatedEntities.Count);

            await _repo.RemoveRangeAsync(_identity, _repo.Entities(_identity));
            await Assert.ThrowsAsync<LogicalSecurityRiskException>(async () => await _repo.SaveChangesAsync(_identity));
        }

        [Fact]
        public async Task LogicalDelete_MockEntity_NoException()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _add(_repo);
            Assert.Equal(2, EfChangeListener.CreatedEntities.Count);

            await _repo.RemoveAsync(_identity, Entity1);
            await _repo.SaveChangesAsync(_identity);

            Assert.Equal(1, _repo.Entities(_identity).Count());
            Assert.Single(EfChangeListener.RemovedEntities);
            // The LogicalDeleting mock entity should just modified
            Assert.False(EfChangeListener.WasOnModifiedCalled);
        }


        [Fact]
        public async Task LogicalDelete_ALL_IgnoreLogicalDeleteAttributes_NoException()
        {
            var _repo = (EfRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));

            await _add(_repo);
            Assert.Equal(2, EfChangeListener.CreatedEntities.Count);

            _repo.IgnoreLogicalDeleteError = true;

            await _repo.RemoveRangeAsync(_identity, _repo.Entities(_identity));
            await _repo.SaveChangesAsync(_identity);

            Assert.Equal(0, _repo.Entities(_identity).Count());
            Assert.Equal(2, EfChangeListener.RemovedEntities.Count);
            // The LogicalDeleting mock entity should just modified
            Assert.False(EfChangeListener.WasOnModifiedCalled);
        }

        [Fact]
        public async Task LogicalDelete()
        {
            var localRepository = (EfRepository<LogicalDeleteEntity, FakeDbContext>)_serviceProvider.GetService(typeof(EfRepository<LogicalDeleteEntity, FakeDbContext>));

            await localRepository.AddAsync(_identity, Entity2);
            await localRepository.AddAsync(_identity, Entity3);
            await localRepository.SaveChangesAsync(_identity);

            Assert.Equal(2, EfChangeListener.CreatedEntities.Count);
            Assert.Equal(2, (await localRepository.EntitiesAsync(_identity)).Count());

            await localRepository.RemoveAsync(_identity, Entity2);
            await localRepository.SaveChangesAsync(_identity);

            Assert.Equal(2, (await localRepository.EntitiesAsync(_identity)).Count());
            var entities = (await localRepository.EntitiesAsync(_identity)).ToArray();
            Assert.Equal(1, (await localRepository.EntitiesAsync(_identity)).Count(entity => entity.IsDeleted && entity.Id == Entity2.Id));
            Assert.Equal(1, (await localRepository.EntitiesAsync(_identity)).Count(entity => !entity.IsDeleted && entity.Id == Entity3.Id));
            Assert.Single(EfChangeListener.RemovedEntities);
            Assert.Empty(EfChangeListener.ModifiedNewEntities);
            Assert.Empty(EfChangeListener.ModifiedOriginalEntities);
            // The LogicalDeleting mock entity should just modified
            Assert.False(EfChangeListener.WasOnModifiedCalled);
            localRepository.Dispose();
        }

        [Fact]
        public async Task LogicalDeleteInheritance()
        {
            var localRepository = (EfRepository<InheritedLogicalDeleteEntity, FakeDbContext>)_serviceProvider.GetService(typeof(EfRepository<InheritedLogicalDeleteEntity, FakeDbContext>));

            await localRepository.AddAsync(_identity, Entity4);
            await localRepository.AddAsync(_identity, Entity5);
            await localRepository.SaveChangesAsync(_identity);

            Assert.Equal(2, EfChangeListener.CreatedEntities.Count);
            Assert.Equal(2, (await localRepository.EntitiesAsync(_identity)).Count());

            await localRepository.RemoveAsync(_identity, Entity4);
            await localRepository.SaveChangesAsync(_identity);

            Assert.Equal(2, (await localRepository.EntitiesAsync(_identity)).Count());
            var entities = (await localRepository.EntitiesAsync(_identity)).ToArray();
            Assert.Equal(1, (await localRepository.EntitiesAsync(_identity)).Count(entity => entity.IsDeleted && entity.Id == Entity2.Id));
            Assert.Equal(1, (await localRepository.EntitiesAsync(_identity)).Count(entity => !entity.IsDeleted && entity.Id == Entity3.Id));
            Assert.Single(EfChangeListener.RemovedEntities);
            Assert.Empty(EfChangeListener.ModifiedNewEntities);
            Assert.Empty(EfChangeListener.ModifiedOriginalEntities);
            // The LogicalDeleting mock entity should just modified
            Assert.False(EfChangeListener.WasOnModifiedCalled);
            localRepository.Dispose();
        }

    }
}
