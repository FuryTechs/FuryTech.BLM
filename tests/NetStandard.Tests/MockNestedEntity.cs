using System.Collections.Generic;

namespace FuryTechs.BLM.NetStandard.Tests
{
    public class MockNestedEntity
    {
        public int Id { get; set; }
        public virtual List<MockEntity> MockEntities { get; set; }
    }
}
