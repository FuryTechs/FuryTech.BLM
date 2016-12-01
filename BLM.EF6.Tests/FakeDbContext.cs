using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLM.Tests;

namespace BLM.EF6.Tests
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbConnection connection) : base(connection, true)
        {

        }

        public virtual DbSet<MockEntity> MockEntities { get; set; }
    }

}
