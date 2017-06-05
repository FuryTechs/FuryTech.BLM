using BLM.NetStandard.Tests;
using Microsoft.EntityFrameworkCore;

namespace BLM.EF7.Tests
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions options) : base(options)
        {

        }
        
        public virtual DbSet<MockEntity> MockEntities { get; set; }
        public virtual DbSet<MockNestedEntity> MockNestedEntities { get; set; }

        public virtual DbSet<MockInterpretedEntity> MockInterpretedEntities { get; set; }
    }

}
