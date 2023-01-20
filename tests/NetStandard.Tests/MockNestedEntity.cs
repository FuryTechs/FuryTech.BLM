using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FuryTechs.BLM.NetStandard.Tests
{
    public class MockNestedEntity
    {
        [Key]
        public int Id { get; set; }
        public virtual List<MockEntity> MockEntities { get; set; }
    }
}
