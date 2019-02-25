using FuryTechs.BLM.NetStandard;
using FuryTechs.BLM.NetStandard.Interfaces;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public class MockInterpretedEntityCreateInterpreter : InterpretBeforeCreate<MockInterpretedEntity>
    {
        public override MockInterpretedEntity DoInterpret(MockInterpretedEntity entity, IContextInfo context)
        {
            entity.MockInterpretedValue = MockInterpretedValue.CreateInterpreted;
            return entity;
        }
    }
}
