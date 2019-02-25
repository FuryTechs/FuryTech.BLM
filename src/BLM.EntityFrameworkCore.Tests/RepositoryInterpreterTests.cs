using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FuryTech.BLM.EntityFrameworkCore.Tests
{
    public class RepositoryInterpreterTests : AbstractEfRepositoryTest
    {
        public RepositoryInterpreterTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task TestCreateInterpret()
        {
            var _repoInterpreted = (EfRepository<MockInterpretedEntity, FakeDbContext>)_serviceProvider.GetService(typeof(EfRepository<MockInterpretedEntity, FakeDbContext>));


            await _repoInterpreted.AddAsync(_identity, new MockInterpretedEntity()
            {
                MockInterpretedValue = MockInterpretedValue.Default
            });
            await _repoInterpreted.SaveChangesAsync(_identity);

            Assert.Equal(1, _repoInterpreted.Entities(_identity).Count());
            Assert.Equal(MockInterpretedValue.CreateInterpreted, _repoInterpreted.Entities(_identity).FirstOrDefault().MockInterpretedValue);
        }

        [Fact]
        public async Task TestModifyInterpret()
        {
            var _db = (FakeDbContext)_serviceProvider.GetService(typeof(FakeDbContext));
            var _repoInterpreted = (EfRepository<MockInterpretedEntity, FakeDbContext>)_serviceProvider.GetService(typeof(EfRepository<MockInterpretedEntity, FakeDbContext>));

            _db.MockInterpretedEntities.Add(new MockInterpretedEntity()
            {
                MockInterpretedValue = MockInterpretedValue.Default
            });
            await _db.SaveChangesAsync();

            var entity = _repoInterpreted.Entities(_identity).FirstOrDefault();
            entity.Index = 2;
            await _repoInterpreted.SaveChangesAsync(_identity);

            Assert.Equal(1, _repoInterpreted.Entities(_identity).Count());
            Assert.Equal(MockInterpretedValue.ModifyInterpreted, _repoInterpreted.Entities(_identity).FirstOrDefault().MockInterpretedValue);
        }
    }
}
