using System.Data.Common;
using System.Data.Entity;
using BLM.Tests;

namespace BLM.EF7.Tests
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbConnection connection) : base(connection, true)
        {

        }

        public virtual DbSet<MockEntity> MockEntities { get; set; }
        public virtual DbSet<MockNestedEntity> MockNestedEntities { get; set; }

        public virtual DbSet<MockInterpretedEntity> MockInterpretedEntities { get; set; }
    }

}
